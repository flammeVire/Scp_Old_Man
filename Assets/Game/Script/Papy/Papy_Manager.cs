using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Photon.Voice;
using System.Threading.Tasks;
using FMOD;
using FMODUnity;
using TMPro;

public class Papy_Manager : NetworkBehaviour
{
    NetworkBool IsFlashed = false;

    public Transform Head;
    [Networked]public NetworkBool CanMove {get; set;} = false;
    public static Papy_Manager Instance;
    [SerializeField] public Papy_State currentState;

    public Transform[] PointToReach;
    
    public NetworkObject PocketPapy;

    public Papy_Vision pVision;
    public Papy_Mouvement pMouvement;
    public Papy_Animation pAnim;

    public GameObject WallPortalPrefab;
    public GameObject WallPortal;
    public NetworkObject Corrosion;
    public GameObject FloorPortalPrefab1;
    public GameObject FloorPortalPrefab2;
    public GameObject FloorPortal;

    public StudioEventEmitter rire;

    public enum Papy_State
    {
        Patrol,
        searching,
        chasing,
    }

    
    private void Start()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        //Rpc_SetAuthorityToPlayerOne();
    }
    public void Update()
    {
        if (HasStateAuthority)
        {
            UpdateState();
            Rpc_UpdatePocketPapyPos();
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    void Rpc_UpdatePocketPapyPos()
    {
        Vector3 localPos = GameManager.Instance.World.transform.InverseTransformPoint(transform.position);
        Vector3 projectedPositionInTargetWorld = GameManager.Instance.PocketWorld.transform.TransformPoint(localPos);
        PocketPapy.transform.position = projectedPositionInTargetWorld;
        PocketPapy.transform.rotation = transform.rotation;
    }
    public void UpdateState()
    {

        if (pVision.canSeePlayer) // si vois le joueur
        {
            Rpc_ChangeStatus(3);
            pMouvement.CurrentPointToReach = pVision.Target.transform.position;
            
        }
        if(currentState == Papy_State.chasing && !pVision.canSeePlayer) 
        {
            UnityEngine.Debug.Log("OUI");
            Rpc_ChangeStatus(2);
            pMouvement.CurrentPointToReach = pVision.LastestTargetPos.position;
            pMouvement.MinimumDistance = 0.5f;

        }
        TryToLookAt(pMouvement.CurrentPointToReach);

    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_FloorSpawnPortals(Vector3 position, Quaternion rotation)
    {
        if (HasStateAuthority)
        {
            Vector3 localPos = GameManager.Instance.World.transform.InverseTransformPoint(transform.position);
            Vector3 projectedPositionInTargetWorld = GameManager.Instance.PocketWorld.transform.TransformPoint(localPos);

            NetworkObject clone = NetworkManager.runnerInstance.Spawn(FloorPortalPrefab1, position, transform.rotation);
            NetworkObject clonePocket = NetworkManager.runnerInstance.Spawn(FloorPortalPrefab2, projectedPositionInTargetWorld, transform.rotation);
        }
    }
    
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_WallSpawnPortals(Vector3 position, Quaternion rotation)
    {
        if (HasStateAuthority)
        {
            if (WallPortal == null)
            {
                Vector3 localPos = GameManager.Instance.World.transform.InverseTransformPoint(transform.position);
                Vector3 projectedPositionInTargetWorld = GameManager.Instance.PocketWorld.transform.TransformPoint(localPos);

                NetworkObject clone = NetworkManager.runnerInstance.Spawn(FloorPortalPrefab1, position, transform.rotation);
                NetworkObject clonePocket = NetworkManager.runnerInstance.Spawn(FloorPortalPrefab2, projectedPositionInTargetWorld, transform.rotation);
                WallPortal = clone.gameObject;
                //NetworkManager.runnerInstance.Despawn(clone);
            }
        }
    }



    public void TryToLookAt(Vector3 targetPosition)
    {
        // Ignore la hauteur pour une rotation horizontale
        targetPosition = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
        Vector3 direction = (targetPosition - transform.position).normalized;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            pMouvement.body.MoveRotation(targetRotation);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_TeleportPapy()
    {
        rire.Stop();
        rire.Play();
        if (HasStateAuthority)
        {
            int randomPoint = UnityEngine.Random.Range(0, PointToReach.Length);
                if (Vector3.Distance(this.transform.position, PointToReach[randomPoint].position) < 5)
                {
                    Rpc_TeleportPapy();
                }
                else
                {
                    transform.position = PointToReach[randomPoint].position;
                    transform.rotation = PointToReach[randomPoint].rotation;
                pAnim.Teleport();
            }
            
        }
    }


    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_TeleportToPreciseLocation(Vector3 TargetPosition, Quaternion rotation)
    {
        rire.Stop();
        rire.Play();
        transform.position = TargetPosition;
        transform.rotation = rotation;
        pAnim.Teleport();
    }

    /*
    
    [Rpc(RpcSources.All,RpcTargets.All)]
    void Rpc_SetAuthorityToPlayerOne()
    {
        if (HasStateAuthority)
        {
            this.GetComponent<NetworkObject>().ReleaseStateAuthority();
        }
        if(LobbyManager.instance.FirstPlayer == NetworkManager.runnerInstance.LocalPlayer)
        {
            this.GetComponent<NetworkObject>().RequestStateAuthority();
        }
    }
  
    */
    [Rpc(RpcSources.All,RpcTargets.All)]
    public void Rpc_ChangeStatus(int Status)
    {
        switch (Status)
        {
            case 1:
                 currentState = Papy_State.Patrol;
                break;
            case 2:
                currentState = Papy_State.searching;
                break;
            case 3:
                currentState = Papy_State.chasing;
                break;
            default:
                currentState = Papy_State.Patrol;
                break;
        }
    }
    
    public void SearchingLocker(NetworkObject obj)
    {
        pMouvement.MinimumDistance = 0.5f;
        pMouvement.CurrentPointToReach = obj.transform.position;
        GetPlayerInLocker(obj);
        Rpc_StopSearching(obj);
    }

    public void GetPlayerInLocker(NetworkObject obj)
    {
        if (obj.GetComponent<HidingSpot>().IsSomeoneHide)
        {
            obj.GetComponent<HidingSpot>().Rpc_ShowPlayerMesh();
        }
    }


    public async void Rpc_StopSearching(NetworkObject obj)
    {
        await Task.Delay(1000);
        if (obj.HasStateAuthority)
        {
            NetworkManager.runnerInstance.Despawn(obj);
            NetworkManager.runnerInstance.Spawn(Papy_Manager.Instance.Corrosion,obj.transform.position, Quaternion.identity);

        }
        pMouvement.MinimumDistance = 0.5f;
        pMouvement.GetAPoint(obj.transform.position);
        Rpc_ChangeStatus(1);
    }
}

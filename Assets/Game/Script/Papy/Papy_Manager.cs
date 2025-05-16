using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Photon.Voice;

public class Papy_Manager : NetworkBehaviour
{
    NetworkBool IsFlashed = false;
    NetworkBool CanPassByWall = false;
    public static Papy_Manager Instance;
    [SerializeField] public Papy_State currentState;

    public Transform[] PointToReach;
    
    public NetworkObject PocketPapy;

    public Papy_Vision pVision;
    public Papy_Mouvement pMouvement;

    public GameObject WallPortalPrefab;
    public GameObject WallPortal;

    public GameObject FloorPortalPrefab1;
    public GameObject FloorPortalPrefab2;
    public GameObject FloorPortal;
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
        UpdateState();
        Rpc_UpdatePocketPapyPos();
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
            //Rpc_ChangeStatus(2);
            currentState = Papy_State.chasing;

            pMouvement.CurrentPointToReach = pVision.Target.transform;
        }
        else if(currentState == Papy_State.chasing) 
        {

        }


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


    public void LookAt(Vector3 TargetPosition)
    {
        TargetPosition = new Vector3(TargetPosition.x,transform.position.y,TargetPosition.z);

        //transform.LookAt(TargetPosition); // si look, n'avance plus 
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_TeleportPapy()
    {
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
                }
            
        }
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_TeleportToPreciseLocation(Vector3 TargetPosition, Quaternion rotation)
    {

        transform.position = TargetPosition;
        transform.rotation = rotation;
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
    
}

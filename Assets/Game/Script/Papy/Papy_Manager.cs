using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Papy_Manager : NetworkBehaviour
{
    NetworkBool IsFlashed = false;
    NetworkBool CanPassByWall = false;
    public static Papy_Manager Instance;
    public Papy_State currentState;

    public Transform[] PointToReach;

    public Papy_Vision pVision;
    public Papy_Mouvement pMouvement;

    public GameObject PortalPrefab;
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
           // currentState = Papy_State.Patrol;
            // Rpc_ChangeStatus(0);
        }

        //ChooseTarget();

    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_SpawnPortals(Vector3 position, Quaternion rotation)
    {if (HasStateAuthority)
        {
        
            Vector3 localPos = GameManager.Instance.World.transform.InverseTransformPoint(transform.position);
            Vector3 projectedPositionInTargetWorld = GameManager.Instance.PocketWorld.transform.TransformPoint(localPos);

            NetworkObject clone = NetworkManager.runnerInstance.Spawn(PortalPrefab, position, transform.rotation);
            NetworkObject clonePocket = NetworkManager.runnerInstance.Spawn(PortalPrefab, projectedPositionInTargetWorld, transform.rotation);
          //  clone.gameObject.transform.position = GameManager.
        }
    }


    public void LookAt(Vector3 TargetPosition)
    {
        TargetPosition = new Vector3(TargetPosition.x,transform.position.y,TargetPosition.z);

        //transform.LookAt(TargetPosition); // si look, n'avance plus 
    }

    void CheckWallCollision()
    {
        
    }




    /*
    void ChooseTarget()
    {
        // check in update wich player have more of IP (interrest point)
    }
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
  
    
    [Rpc(RpcSources.All,RpcTargets.All)]
    void Rpc_ChangeStatus(int Status)
    {
        switch (Status)
        {
            case 0:
                 currentState = Papy_State.Patrol;
                break;
            case 1:
                currentState = Papy_State.searching;
                break;
            case 2:
                currentState = Papy_State.chasing;
                break;
            default:
                currentState = Papy_State.Patrol;
                break;
        }
    }
    */ //pas utile pour le moment
}

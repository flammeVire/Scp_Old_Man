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
    
    

    public override void FixedUpdateNetwork()
    {
        
    }

    public void LookAt(Vector3 TargetPosition)
    {
        TargetPosition = new Vector3(TargetPosition.x,transform.position.y,TargetPosition.z);

        transform.LookAt(TargetPosition);
    }
    void ChooseTarget()
    {
        
    }
}

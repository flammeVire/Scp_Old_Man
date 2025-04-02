using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Papy_Manager : NetworkBehaviour
{
    NetworkBool IsFlashed = false;
    NetworkBool CanPassByWall = false;

    public Papy_State currentState;
    public enum Papy_State
    {
        wandering,
        searching,
        chasing,
    }


    private void Start()
    {
        Rpc_SetAuthorityToPlayerOne();
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

    void ChooseTarget()
    {
        
    }
}

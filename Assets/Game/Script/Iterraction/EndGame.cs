using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EndGame : NetworkBehaviour
{
    public bool CanGameEnd;
    public GameObject DoorPrefab;
    public Transform Spawn;
    public NetworkObject Door;
    public float Delay;
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_ClosingEndSas(NetworkBool Closing)
    {
        if (HasStateAuthority)
        {
            if (Closing)
            {
                Debug.Log("Closing endSas");
                Door = NetworkManager.runnerInstance.Spawn(DoorPrefab, Spawn.position, Spawn.rotation);
                //MattSounds : jouer son ouverture

                if(IsGameFinish()) 
                {
                    Rpc_FinishGame();
                }
            }
            else
            {
                Debug.Log("opening endSas");
                NetworkManager.runnerInstance.Despawn(Door);

                Door = null;
                //MattSounds : Jouer son fermeture
            }
        }
    }

    public bool IsGameFinish()
    {
        if (Prison.IsInside)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    [Rpc(RpcSources.All,RpcTargets.All)]
    public void Rpc_FinishGame()
    {
        //METTRE UN TRUC A LA FIN
        Debug.Log("Game IS finish ");
    }
}

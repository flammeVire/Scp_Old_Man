using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Sas_Door : NetworkBehaviour
{
    public GameObject DoorPrefab;
    public NetworkObject Door;
    public Transform Spawn;

    public float Delay;

    [Rpc(RpcSources.All,RpcTargets.All)]
    public void Rpc_OpeningSas(NetworkBool IsOpen)
    {
        if (HasStateAuthority)
        {
            if (Door != null)
            {
                NetworkManager.runnerInstance.Despawn(Door);
                Door = null;
            }
            else
            {
                Door = NetworkManager.runnerInstance.Spawn(DoorPrefab,Spawn.position,Spawn.rotation);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Sas_Door : NetworkBehaviour
{
    public GameObject DoorPrefab;
    public NetworkObject Door;
    public Transform Spawn;

    [SerializeField] Transform Button1;
    [SerializeField] BoxCollider Collider1;
    [SerializeField] Transform Button2;
    [SerializeField] BoxCollider Collider2;


    public float Delay;

    public void OnValidate()
    {
        Collider1.center = Button1.localPosition;
        Collider1.size = Button1.localScale;
        Collider2.center = Button2.localPosition;
        Collider2.size = Button2.localScale;
    }


    [Rpc(RpcSources.All,RpcTargets.All)]
    public void Rpc_OpeningSas(NetworkBool IsOpen)
    {
        if (HasStateAuthority)
        {
            if (Door != null)
            {
                NetworkManager.runnerInstance.Despawn(Door);
                //MattSounds : jouer son ouverture
                
                Door = null;
            }
            else
            {
                Door = NetworkManager.runnerInstance.Spawn(DoorPrefab,Spawn.position,Spawn.rotation);

                //MattSounds : Jouer son fermeture
            }
        }
    }
}

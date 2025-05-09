using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Unity.VisualScripting;

public class SoundDoor : NetworkBehaviour
{
    [Range(0,10)]public int SoundRequired;
    public int ActualSound;
    public Operation operation;
    public NetworkObject Door;
    bool IsOpen;

    bool SomeoneInside;

    
    List<GameObject> PlayerInside = new List<GameObject>();

    public enum Operation
    {
        Greater,
        Lower,
        Equal,
    }

    private void OnTriggerEnter(Collider other)
    {
        SomeoneInside = true;
        PlayerInside.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerInside.Remove(other.gameObject);
        if (PlayerInside.Count <= 0)
        {
            SomeoneInside = false;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (SomeoneInside) 
        {
            Debug.Log("SomeoneInside");
            CheckPlayerPI();
        }
    }

    public void CheckPlayerPI()
    {
        int globalPI = 0;
        foreach (GameObject go in PlayerInside)
        {
            globalPI += go.GetComponent<PlayerInterrestPoint>().CurrentPI;
        }
        Debug.Log("global PI inDoor = " + globalPI);
        if (checkOperation(globalPI))
        {
            Debug.Log("Open the door");
            Rpc_OpenDoor();

            //MattSounds : mettre fonction RPC qui joue un son si le volume dépasse un certain seuil (PI va de 0 a 10)
        }
    }

    bool checkOperation(int globalPI)
    {

        switch (operation)
        {
            case Operation.Greater:
                if (globalPI >= SoundRequired)
                {
                    return true;
                }
                break;
            case Operation.Lower:
                if (globalPI <= SoundRequired)
                {
                    return true;
                }
                break;
            case Operation.Equal:
                if (globalPI == SoundRequired)
                {
                    return true;
                }
                break;
        }
        return false;
    }

    [Rpc(RpcSources.All,RpcTargets.All)]
    void Rpc_OpenDoor()
    {
        if (HasStateAuthority)
        {
            NetworkManager.runnerInstance.Despawn(Door);
        }
    }
    
}

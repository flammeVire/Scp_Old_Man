using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine.ProBuilder.Shapes;
public class Double_Door : NetworkBehaviour
{
    public ButtonDouble_Door[] ButtonScript;
    public GameObject DoorPrefabs;
    public NetworkObject Door;
    public NetworkBool IsOpen;
    public Transform spawn;

    public void CheckAllButton()
    {
        foreach (ButtonDouble_Door button in ButtonScript)
        {
            if (!button.ButtonActive)
            {
                Debug.Log("All Button are not enable");
                return;
            }
        }
        Rpc_ManageOpening(true);
    }


    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_ManageOpening(bool Activate)
    {
        Debug.Log("Door ==" + Door);
        if (Activate)
        {
            if (Door.HasStateAuthority)
            {
                Debug.Log("Have Authority");
                NetworkManager.runnerInstance.Despawn(Door);
                IsOpen = true;
            }
        }
        else
        {
            Debug.Log("Spawning");
            Door = NetworkManager.runnerInstance.Spawn(DoorPrefabs, spawn.position, spawn.rotation);
            IsOpen = false;
        }
    }
}

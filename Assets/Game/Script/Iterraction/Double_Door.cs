using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine.ProBuilder.Shapes;
using FMODUnity;
public class Double_Door : NetworkBehaviour
{
    public ButtonDouble_Door[] ButtonScript;
    public GameObject DoorPrefabs;
    public NetworkObject Door;
    public NetworkBool IsOpen;
    public Transform spawn;

    public StudioEventEmitter doorSound;

    public void CheckAllButton()
    {
        bool CanOpen = true;

      //  Debug.Log("Nb of button" + ButtonScript.Length);
        foreach (ButtonDouble_Door button in ButtonScript)
        {
            Debug.Log("button is " + button.gameObject.name);
            if (!button.ButtonActive)
            {
                Debug.Log("All Button are not enable");
                Debug.Log(button.gameObject.name + " is the issues");
                CanOpen = false;
                break;
            }
        }
        Rpc_ManageOpening(CanOpen);
    }


    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_ManageOpening(bool Activate)
    {
        Debug.Log("Door ==" + Door);
        if (Door.HasStateAuthority)
        {
            if (Activate)
            {
                Debug.Log("Have Authority");
                NetworkManager.runnerInstance.Despawn(Door);
                //MattSounds : jouer son ouverture
                doorSound.Play();
                IsOpen = true;
            }
        }
    }
}

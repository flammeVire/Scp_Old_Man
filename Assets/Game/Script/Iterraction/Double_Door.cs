using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine.ProBuilder.Shapes;
using FMODUnity;
using System.Threading.Tasks;
using FMOD;
public class Double_Door : NetworkBehaviour
{
    public ButtonDouble_Door[] ButtonScript;
    public NetworkObject Door1;
    public NetworkObject Door2;
    public NetworkBool IsOpen;
    public Animation anim;

    public DoorsSound doorSound;

    public void CheckAllButton()
    {
        bool CanOpen = true;

      //  Debug.Log("Nb of button" + ButtonScript.Length);
        foreach (ButtonDouble_Door button in ButtonScript)
        {
            if (!button.ButtonActive)
            {
                CanOpen = false;
                break;
            }
        }
        Rpc_ManageOpening(CanOpen);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public async void Rpc_ManageOpening(bool Activate)
    {
        
        if (Activate)
        {
            if (!anim.isPlaying)
            {
                //if (Door1.gameObject != null)
                //{
                    doorSound.Rpc_Open();
               // }
                anim.Play();
                anim[anim.clip.name].speed = 1;              // Joue l’animation en sens inverse
                anim[anim.clip.name].time = 0;
            }

            await WaitForAnimationEnd();

            if (Door1.HasStateAuthority)
            {

                NetworkManager.runnerInstance.Despawn(Door1);

                //MattSounds : jouer son ouverture
                IsOpen = true;
            }
            if (Door2.HasStateAuthority)
            {
                NetworkManager.runnerInstance.Despawn(Door2);
                IsOpen = true;
            }
        }

    }
    private async Task WaitForAnimationEnd()
    {
        // Attend que l'animation finisse
        while (anim.isPlaying)
        {
            await Task.Yield(); // Attend la frame suivante
        }
    }

}

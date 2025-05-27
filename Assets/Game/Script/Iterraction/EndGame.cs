using FMOD;
using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
public class EndGame : NetworkBehaviour
{
    [Networked] public NetworkBool CanGameEnd { get; private set; }

    public Animation anim;
    public float Delay;

    [Rpc(RpcSources.All, RpcTargets.All)]
    public async void Rpc_ClosingEndSasAsync()
    {
        /*
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
        */
        // if (HasStateAuthority)

        if (!anim.isPlaying)
        {
            //sound.Rpc_Open();
            anim.Play();
            anim[anim.clip.name].speed = 1;              // Joue l’animation en sens inverse
            anim[anim.clip.name].time = 0;
        }

        await WaitForAnimationEnd();

        if (IsGameFinish())
        {
            Rpc_FinishGame();
        }

        else if (!anim.isPlaying)
        {

            anim.Play();
            anim[anim.clip.name].speed = -1;              // Joue l’animation en sens inverse
            anim[anim.clip.name].time = anim[anim.clip.name].length;
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

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_FinishGame()
    {
        //METTRE UN TRUC A LA FIN
        UnityEngine.Debug.Log("Game IS finish ");
        SceneManager.LoadScene(3);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Unity.Properties;
using System.Threading.Tasks;
using FMOD;

public class Card_Door : NetworkBehaviour
{
    public NetworkObject Door1;
    public NetworkObject Door2;
    public NetworkBool IsOpen;
    public bool needCard;
    public DoorsSound doorsSound;
    public Animation anim;
    [Range(1, 3)] public int CardLvL;

    public void Interract(PickItem player)
    {
        if (!IsOpen)
        {
            if (needCard)
            {
                CardAccess(player);
            }
            else
            {
                FreeAccess();
            }
        }
    }

    void CardAccess(PickItem player)
    {
        if (CheckPlayerCard(player))
        {
            if (Door1.gameObject != null)
            {
                doorsSound.Rpc_Good();
            }

            Rpc_ManageOpening();
        }
        else
        {
            if (Door1.gameObject != null)
            {
                doorsSound.Rpc_Bad();
            }
        }
    }

    void FreeAccess()
    {
        Rpc_ManageOpening();
    }

    bool CheckPlayerCard(PickItem player)
    {
        switch (CardLvL)
        {
            case 1:
                if (player.HaveCardLvL1)
                {
                    return true;
                }
                break;
            case 2:
                if (player.HaveCardLvL2)
                {
                    return true;
                }
                break;
            case 3:
                if (player.HaveCardLvL3)
                {
                    return true;
                }
                break;
        }
        return false;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public async void Rpc_ManageOpening()
    {
        /*
        if (!IsOpen)
        {
            doorsSound.Rpc_Open();
            if (Door.HasStateAuthority)
            {

                Debug.Log("Have Authority");
                NetworkManager.runnerInstance.Despawn(Door);
                //MattSounds : jouer son ouverture
                IsOpen = true;
            }
        }
        */
        if (!anim.isPlaying)
        {
            if (Door1.gameObject != null)
            {
                doorsSound.Rpc_Open();
            }
            
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
    private async Task WaitForAnimationEnd()
    {
        // Attend que l'animation finisse
        while (anim.isPlaying)
        {
            await Task.Yield(); // Attend la frame suivante
        }
    }

}

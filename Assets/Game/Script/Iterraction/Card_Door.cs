using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Unity.Properties;

public class Card_Door : NetworkBehaviour
{
    public GameObject DoorPrefabs;
    public NetworkObject Door;
    public NetworkBool IsOpen;
    public Transform spawn;
    public bool needCard;
    public DoorsSound doorsSound;
    [Range(1,3)]public int CardLvL;

    public void Interract(PickItem player)
    {
        if (needCard)
        {
            Debug.Log("Card Need");
            CardAccess(player);
        }
        else
        {
            Debug.Log("Free access");
            FreeAccess();
        }
    }

    void CardAccess(PickItem player)
    {
        if (CheckPlayerCard(player))
        {
            Debug.Log("PlayerHaveCard");
            doorsSound.Rpc_Good();
            Rpc_ManageOpening();
        }
        else 
        {
            doorsSound.Rpc_Bad();
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
    void Rpc_ManageOpening()
    {
        Debug.Log("Door ==" + Door);
        Debug.Log("Is Open is " + IsOpen);

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
    } 
}

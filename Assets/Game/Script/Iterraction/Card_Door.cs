using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Card_Door : MonoBehaviour
{
    public GameObject DoorPrefabs;
    public NetworkObject Door;
    public NetworkBool IsOpen;
    public Transform spawn;
    public bool needCard;
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
            ManageOpening();
        }
    }

    void FreeAccess()
    {
        ManageOpening();
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

    void ManageOpening()
    {
        if (!IsOpen)
        {
            NetworkManager.runnerInstance.Despawn(Door);
        }
        else
        {
            Door = NetworkManager.runnerInstance.Spawn(DoorPrefabs, spawn.position, spawn.rotation);
        }
    } 
}

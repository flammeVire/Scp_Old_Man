using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System.Collections;


public class PickItem : NetworkBehaviour
{
    public PlayerMouvement mouvement;
    public float pickupRange = 2f;
    public Camera playerCamera;

    [Header("Flash")]
    public GameObject GrenadePrefab;
    public int NumberOfFlashGrenade = 0;

    [Header("Card")]
    public bool HaveCardLvL1;
    public bool HaveCardLvL2;
    public bool HaveCardLvL3;
    public GameObject[] CardMesh;

    private void Start()
    {

    }
    void Update()
    {
        if (HasInputAuthority)
        {
            if (Input.GetButtonDown("Interract"))
            {
                Debug.Log("Interract");
                TryPickupItem();
            }

            else if (Input.GetButtonDown("DropItem"))
            {
                Debug.Log("Drop");
                DropItem();
            }
        }
    }
    /*
    #region manage
    void ChangeItem()
    {
        switch (index)
        {
            case 0:
                index = 1;
                break;
            case 1:
                index = 0;
                break;
            default:
                index = 0;
                break;
        }
        CurrentItem = inventory[index];
    }

    
    void ShowItem(Item item)
    {
        if (item != null)
        {
            NetworkSpawnOp obj = NetworkManager.runnerInstance.SpawnAsync(item.HandMesh, hand.transform.position, hand.transform.rotation, NetworkManager.runnerInstance.LocalPlayer);
            Rpc_SyncTransform(obj.Object);
            obj.Object.transform.parent = hand.transform;
        }
    }

    void RemoveObj()
    {
        NetworkObject currentItem = hand.GetComponentInChildren<NetworkObject>();
        if (currentItem != null)
        {
            NetworkManager.runnerInstance.Despawn(currentItem);
        }
    }
    
    [Rpc(RpcSources.All, RpcTargets.All)]
    void Rpc_SyncTransform(NetworkObject obj)
    {
        obj.transform.position = hand.transform.position;
        obj.transform.rotation = hand.transform.rotation;
        obj.transform.parent = hand.transform;
        //obj.name = CurrentItem.Name;
    }
    
    #endregion
    */
    #region pick

    void TryPickupItem()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickupRange))
        {
            if (hit.collider.CompareTag("Item"))
            {
                NetworkObject netObj = hit.collider.GetComponent<NetworkObject>();
                if (netObj != null)
                {
                    Debug.Log("Tentative de prise d'autorité...");
                    Rpc_RequestAndPickup(netObj,NetworkManager.runnerInstance.LocalPlayer, "Item");
                }
            }
            else if (hit.collider.CompareTag("Card"))
            {
                NetworkObject netObj = hit.collider.GetComponent<NetworkObject>();
                if (netObj != null)
                {
                    if (!AlreadyHaveCard(netObj))
                    {
                        Debug.Log("Tentative de prise d'autorité...");
                        Rpc_RequestAndPickup(netObj, NetworkManager.runnerInstance.LocalPlayer, "Card");
                    }
                }
            }
        }
    }

    [Rpc( RpcSources.All, RpcTargets.All)]
    void Rpc_RequestAndPickup(NetworkObject netObj,PlayerRef player,string Type)
    {
        Debug.Log("RPC_Request for " + netObj + "to +" +  player);
        if (player == NetworkManager.runnerInstance.LocalPlayer)
        {
            if (Type == "Item")
            {
                Debug.Log("Player is the local player");
                PickupItem();
            }
            else if(Type =="Card")
            {
                PickUpCard(netObj.name);
            }
        }
        if (netObj.HasStateAuthority)
        {
            Debug.Log("Have Authority : despawn");
            NetworkManager.runnerInstance.Despawn(netObj.GetComponent<NetworkObject>()); // Despawn l'objet du réseau
        }
    }

    void PickupItem()
    {
        
        NumberOfFlashGrenade++;
        mouvement.playerUI.FlashText.text = NumberOfFlashGrenade.ToString();
        Debug.Log("pickUpFinished");
    }

    void PickUpCard(string objName)
    {

        if (objName == CardMesh[0].name)
        {
            HaveCardLvL1 = true;
            mouvement.playerUI.Card1.gameObject.SetActive(true);
        }
        else if (objName == CardMesh[1].name)
        {
            HaveCardLvL2 = true;
            mouvement.playerUI.Card2.gameObject.SetActive(true);

        }
        else if(objName == CardMesh[2].name)
        {
            HaveCardLvL3 = true;
            mouvement.playerUI.Card3.gameObject.SetActive(true);

        }

    }

    bool AlreadyHaveCard(NetworkObject obj)
    {

        if (obj.gameObject.name == CardMesh[0].name && HaveCardLvL1)
        {
            return true;
        }
        else if (obj.gameObject.name == CardMesh[1].name && HaveCardLvL2)
        {
            return true;
        }
        else if (obj.gameObject.name == CardMesh[2].name && HaveCardLvL3)
        {
            return true;
        }
        return false;
    }
    #endregion
    #region drop

    void DropItem()
    {
        if (NumberOfFlashGrenade > 0)
        { 
            NetworkSpawnOp obj = NetworkManager.runnerInstance.SpawnAsync(GrenadePrefab, transform.position, transform.rotation, NetworkManager.runnerInstance.LocalPlayer);
            NumberOfFlashGrenade--;
            mouvement.playerUI.FlashText.text = NumberOfFlashGrenade.ToString();
        }

    }


    #endregion
}

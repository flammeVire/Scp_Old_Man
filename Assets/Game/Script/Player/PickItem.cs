using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System.Collections;


public class PickItem : NetworkBehaviour
{
    public float pickupRange = 2f;
    public Camera playerCamera;
    public Transform hand;

    public Item CurrentItem;
    public Item[] inventory = new Item[2];
    public List<Item> PossiblesItem;
    int index = 0;

    public bool HaveCardLvL1,HaveCardLvL2,HaveCardLvL3;


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
            else if (Input.GetButtonDown("Fire1"))
            {
                Debug.Log("Throw");
                Throw();
            }
            else if (Input.GetAxis("Mouse ScrollWheel") != 0f)
            {
                Debug.Log("index: " + index + " | item selected is: " + inventory[index]);
                RemoveObj();
                ChangeItem();
                ShowItem(inventory[index]);
            }
        }
    }

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
                    Rpc_RequestAndPickup(netObj,NetworkManager.runnerInstance.LocalPlayer);
                }
            }
        }
    }

    [Rpc( RpcSources.All, RpcTargets.All)]
    void Rpc_RequestAndPickup(NetworkObject netObj,PlayerRef player)
    {
        Debug.Log("RPC_Request for " + netObj + "to +" +  player);
        if (player == NetworkManager.runnerInstance.LocalPlayer) 
        {
            Debug.Log("Player is the local player");
            PickupItem(netObj.gameObject.name);
        }
        if (netObj.HasStateAuthority)
        {
            Debug.Log("Have Authority : despawn");
            NetworkManager.runnerInstance.Despawn(netObj.GetComponent<NetworkObject>()); // Déspawn l'objet du réseau
        }
        else
        {
            Debug.Log("Have not Authority : do nothing");
        }
    }

    void PickupItem(string item)
    {
        if (inventory[index] == null && item != null)
        {
            Debug.Log("inventory item = " +inventory[index]);
            Debug.Log("item = " + item);
            foreach (Item possibleItem in PossiblesItem)
            {
                Debug.Log("possibleItem = " + possibleItem);
                Debug.Log("possibleItem Name= " + possibleItem.Name + "| item Name" + item );
                if (possibleItem != null)
                {
                    Debug.Log("PossibleItem != null");
                    if (possibleItem.Name == item)
                    {
                        Debug.Log(possibleItem.Name + " == " +item);
                        inventory[index] = possibleItem;
                        CurrentItem = possibleItem;
                        ShowItem(possibleItem);
                        break;
                    }
                    else if (possibleItem.Name + "(Clone)" == item)
                    {
                        Debug.Log(possibleItem.Name + "(Clone)" + " == " + item );
                        inventory[index] = possibleItem;
                        CurrentItem = possibleItem;
                        ShowItem(possibleItem);
                        break;

                    }
                    else
                    {
                        Debug.Log("Name incorrect");
                    }
                }
                else
                {
                    Debug.Log("item non ramassé");
                }
            }

            Debug.Log("pickUpFinished");

        }
    }
    #endregion
    #region drop

    void DropItem()
    {
        if (CurrentItem != null)
        {
            RemoveObj();
            NetworkSpawnOp obj = NetworkManager.runnerInstance.SpawnAsync(CurrentItem.FloorMesh, hand.transform.position, hand.transform.rotation, NetworkManager.runnerInstance.LocalPlayer);
            //obj.Object.name = CurrentItem.Name;
            RemoveItemFromInventory();
        }
    }

    void RemoveItemFromInventory()
    {
        inventory[index] = null;
        CurrentItem = null;
    }

    #endregion
    #region throw

    void Throw()
    {
        if (CurrentItem != null)
        {
            RemoveObj();
            NetworkSpawnOp obj = NetworkManager.runnerInstance.SpawnAsync(CurrentItem.FloorMesh, hand.position, hand.rotation, NetworkManager.runnerInstance.LocalPlayer);
           // obj.Object.name = CurrentItem.Name;

            // Ajoute une force pour le lancer
            Rigidbody rb = obj.Object.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(playerCamera.transform.forward * 10f, ForceMode.Impulse); // Lancer vers l'avant
            }
            RemoveItemFromInventory();

        }
    }
    #endregion
}

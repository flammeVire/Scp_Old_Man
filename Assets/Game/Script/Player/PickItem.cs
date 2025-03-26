using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System.Collections;

public class PlayerInteraction : NetworkBehaviour
{
    public float pickupRange = 2f;
    public Camera playerCamera;
    public Transform hand;
    public Item CurrentItem;
    public Item[] inventory = new Item[2];
    public List<Item> PossiblesItem;
    int index = 0;

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
                    RequestAndPickup(netObj);
                }
            }
        }
    }

    void RequestAndPickup(NetworkObject netObj)
    {
        if (netObj.HasStateAuthority)
        {
            netObj.RequestStateAuthority(); // Demande l'autorité
        }

        StartCoroutine(WaitForAuthorityAndPickup(netObj)); // Attendre l'autorité avant d'agir
    }

    IEnumerator WaitForAuthorityAndPickup(NetworkObject netObj)
    {
        while (!netObj.HasStateAuthority) // Attendre que l'autorité soit transférée
        {
            yield return null;
        }
        Debug.Log("Autorité obtenue, ramassage...");
        PickupItem(netObj.gameObject);
    }

    void PickupItem(GameObject item)
    {
        if (inventory[index] == null && item != null)
        {

            foreach (Item possibleItem in PossiblesItem)
            {
                if (possibleItem != null && possibleItem.Name == item.name)
                {
                    inventory[index] = possibleItem;
                    break;
                }
            }

            Debug.Log("Objet ramassé : " + item.name);
            Runner.Despawn(item.GetComponent<NetworkObject>()); // Déspawn l'objet du réseau

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
            obj.Object.name = CurrentItem.Name;
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
            obj.Object.name = CurrentItem.Name;

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

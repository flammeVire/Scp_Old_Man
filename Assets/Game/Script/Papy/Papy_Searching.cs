using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Papy_Searching : MonoBehaviour
{
    public bool HidingSpot_OnRaduis;
    [Range(0,100)]public int SearchingChance;
    
    [SerializeField] Papy_Manager pManager;
    [SerializeField] Papy_Mouvement pMouvement;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Hide")
        {
            if (pManager.currentState == Papy_Manager.Papy_State.searching && WantToSearch())
            {
                HidingSpot_OnRaduis = true;
                pManager.SearchingLocker(other.GetComponent<NetworkObject>());
            }
            else if(pManager.currentState == Papy_Manager.Papy_State.chasing && pManager.pVision.canSeePlayer == false)
            {
                HidingSpot_OnRaduis = true;
                pManager.SearchingLocker(other.GetComponent<NetworkObject>());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
       // HidingSpot_OnRaduis = false;
    }


    bool WantToSearch() 
    {
        int random = UnityEngine.Random.Range(0,100);
        
        if(random <= SearchingChance)
        {
            Debug.Log("I WANT TO SEARCH");
            return true;
        }
        else
        {
            Debug.Log("Dont want to search :(");
            return false;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Papy_PocketWorld : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 6)
        {
            Debug.Log("Enter In Wall");
            Papy_Manager.Instance.Rpc_SpawnPortals(transform.position,transform.rotation);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        
    }
}

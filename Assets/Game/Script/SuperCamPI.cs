using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class SuperCamPI : NetworkBehaviour
{

    public GameObject obj;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 7)
        {
            Rpc_CurrentTaget();
        }
    }

    [Rpc(RpcSources.All,RpcTargets.All)]
    void Rpc_CurrentTaget()
    {
        Papy_Manager.Instance.pMouvement.CurrentPointToReach = obj.transform.position;
    }

    private void OnTriggerExit(Collider other)
    {
        
    }
}

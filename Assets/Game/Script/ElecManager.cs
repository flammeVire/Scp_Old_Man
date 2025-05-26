using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElecManager : NetworkBehaviour
{
    public bool ComplexeHaveElectricity;
    public PorteCoupeFeu[] porte;
    public GameObject[] Light;
    public static ElecManager instance;

    private void Start()
    {
        instance = this;
        Rpc_ActivateElectricity();
    }

    [Rpc(RpcSources.All,RpcTargets.All)]
    void Rpc_ActivateDoor()
    {
        ComplexeHaveElectricity = false;
        foreach (PorteCoupeFeu feu in porte)
        {
            feu.Rpc_Close();
        }
        foreach (GameObject l in Light)
        {
            l.SetActive(false);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_ActivateElectricity()
    {
        if (!ElecManager.instance.ComplexeHaveElectricity)
        {
            ElecManager.instance.ComplexeHaveElectricity = true;
            foreach (PorteCoupeFeu feu in porte)
            {
                feu.Rpc_Open();
            }
            foreach(GameObject l in Light)
            {
                l.SetActive(true);
            }
        }
    }


private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 10)
        {
            Rpc_ActivateDoor();
        }
    }
}

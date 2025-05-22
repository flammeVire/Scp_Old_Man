using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElecManager : NetworkBehaviour
{
    public NetworkBool ComplexeHaveElectricity;
    PorteCoupeFeu[] porte;

    public static ElecManager instance;

    private void Start()
    {
        instance = this;
    }

    void ActivateDoor()
    {
        foreach (PorteCoupeFeu feu in porte)
        {
            feu.Rpc_Close();
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 10)
        {
            ActivateDoor();
            ComplexeHaveElectricity = false;
        }
    }
}

using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElecManager : NetworkBehaviour
{
    public NetworkBool ComplexeHaveElectricity;
    PorteCoupeFeu[] porte;

    void ActivateDoor()
    {
        foreach (PorteCoupeFeu feu in porte)
        {
            feu.Close();
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

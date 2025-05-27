using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Prison : MonoBehaviour
{
    [Networked]public static NetworkBool IsInside {  get; private set; }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 10)
        {
            Debug.Log("Papy in cell");
            IsInside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 10)
        {
            IsInside = false;
        }
    }
}

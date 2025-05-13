using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class WallHoleCutter : MonoBehaviour
{
    public LayerMask DefaultLayer, PassHoleLayer;

    private void OnTriggerEnter(Collider other)
    {
            Debug.Log("other enter ");
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("other enter + is player");
            if (other.GetComponent<Rigidbody>() != null) 
            {
                Debug.Log("other enter + have Rb");
                Rigidbody rb = other.GetComponent<Rigidbody>();
                rb.excludeLayers = PassHoleLayer;
            }
        }
    }

    private void OnTriggerExit(Collider other) 
    {
            Debug.Log("other exit ");
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("other Exit + is player");
            if (other.GetComponent<Rigidbody>() != null)
            {
                Debug.Log("other exit + have Rb");
                Rigidbody rb = other.GetComponent<Rigidbody>();
                rb.excludeLayers = DefaultLayer;
            }
        }
    }
}


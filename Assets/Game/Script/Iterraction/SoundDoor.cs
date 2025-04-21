using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class SoundDoor : NetworkBehaviour
{
    [Range(0,10)]public int SoundRequired;
    public int ActualSound;

    NetworkObject Door;
    bool IsOpen;

    bool SomeoneInside;
    List<GameObject> PlayerInside = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        SomeoneInside = true;
        PlayerInside.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerInside.Remove(other.gameObject);
        if (PlayerInside.Count <= 0)
        {
            SomeoneInside = false;
        }
    }

    private void FixedUpdate()
    {
        if (SomeoneInside) 
        {
            Debug.Log("SomeoneInside");
        }
    }

    
}

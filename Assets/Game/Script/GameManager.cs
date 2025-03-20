using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class GameManager : NetworkBehaviour
{
    public GameObject PlayerMesh;
    public Transform PlayerTransform;
    private void Start()
    {
        SpawnPlayer();
    }

    public void SpawnPlayer()
    {

    }
}

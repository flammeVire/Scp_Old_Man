using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class GameManager : NetworkBehaviour
{
    public GameObject PlayerMesh;
    public GameObject CameraPrefab;
    public Transform PlayerTransform;
    public static GameManager Instance;
    private void Start()
    {
        SpawnPlayer();
    }

    public void SpawnPlayer()
    {

        NetworkManager.runnerInstance.SpawnAsync(PlayerMesh,PlayerTransform.position,PlayerTransform.rotation,NetworkManager.runnerInstance.LocalPlayer);
    }

    public void Update()
    {
        
    }


}

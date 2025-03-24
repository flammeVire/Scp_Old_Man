using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class GameManager : NetworkBehaviour
{
    public GameObject PlayerMesh;
    public GameObject CameraPrefab;
    public Transform PlayerTransform;
    
    private void Start()
    {
        SpawnPlayer();
    }

    public void SpawnPlayer()
    {
        if (NetworkManager.runnerInstance.IsServer)
        {
            foreach (var player in LobbyManager.instance.playerList)
            {
                NetworkSpawnOp Mesh = NetworkManager.runnerInstance.SpawnAsync(PlayerMesh, PlayerTransform.position, PlayerTransform.rotation, player.playerRef);
                Debug.Log("Assign authority to " + player.playerRef);
            }
        }
    }

   
}

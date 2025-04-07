using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class GameManager : NetworkBehaviour
{
    public GameObject PlayerMeshPrefab;
    public GameObject CameraPrefab;
    public Transform PlayerTransform;

    public GameObject[] PlayerMeshes;
    public static GameManager Instance;
    private void Start()
    {
        SpawnPlayer();
    }

    public void SpawnPlayer()
    {

        NetworkManager.runnerInstance.SpawnAsync(PlayerMeshPrefab,PlayerTransform.position,PlayerTransform.rotation,NetworkManager.runnerInstance.LocalPlayer);
    }

    public void GetAllMeshes()
    {
        PlayerMouvement[] movements = FindObjectsByType<PlayerMouvement>(FindObjectsSortMode.None);
        for(int i =0; i < movements.Length;i++)
        {
            PlayerMeshes[i]= movements[i].gameObject;
        }
    }
}

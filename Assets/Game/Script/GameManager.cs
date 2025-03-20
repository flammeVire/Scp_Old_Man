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
        PlayerManager[] manager = FindObjectsByType<PlayerManager>(FindObjectsSortMode.None);
        
        Debug.Log("manager total find : " +manager.Length);
        foreach(var m in manager)
        {
            Debug.Log(m);
            m.mesh = PlayerMesh;
            m.SpawnPoint = PlayerTransform;
            m.Init();
        }
    }
}

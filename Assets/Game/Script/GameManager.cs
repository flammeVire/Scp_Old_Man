using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System.Linq;

public class GameManager : NetworkBehaviour
{
    public GameObject PlayerMeshPrefab;
    public GameObject CameraPrefab;
    public Transform[] PlayerSpawn;

    public GameObject[] PlayerMeshes;
    public static GameManager Instance;
    private void Start()
    {
        Instance = this;
        SpawnPlayer();
    }

    public void SpawnPlayer()
    {

       // NetworkManager.runnerInstance.SpawnAsync(PlayerMeshPrefab, PlayerSpawn[0].position, PlayerSpawn[0].rotation,NetworkManager.runnerInstance.LocalPlayer);
         NetworkManager.runnerInstance.SpawnAsync(PlayerMeshPrefab,Vector3.zero,Quaternion.identity,NetworkManager.runnerInstance.LocalPlayer);
       
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_GetAllMeshes()
    {
        Debug.Log("GetAllMesh");
        PlayerMouvement[] movements = FindObjectsByType<PlayerMouvement>(FindObjectsSortMode.None);

        Debug.Log("All mesh movements count = " + movements.Length);
        PlayerMeshes = new GameObject[4];
        for (int i = 0; i < movements.Length; i++)
        {
            Debug.Log("i : " + i);
            Debug.Log("PlayerMeshes[i] " + PlayerMeshes[i]);
            Debug.Log("movements[i].gameObject " + movements[i].gameObject);


            PlayerMeshes[i] = movements[i].gameObject;

        }

        StartCoroutine(TeleportAllMeshes(movements));
    }

    IEnumerator TeleportAllMeshes(PlayerMouvement[] movements)
    {
        yield return new WaitForSeconds(5f);
        if (HasStateAuthority)
        {
            for(int i = 0; i < PlayerMeshes.Length; i++)
            {
                if(movements[i] != null)
                {
                    Debug.Log("Teleporting " + movements[i].name);
                    movements[i].Rpc_TeleportMesh(PlayerSpawn[i].position, PlayerSpawn[i].rotation);
                }
            }
        }
    }

    public int NumberOfMesh()
    {
         
        PlayerMouvement[] movements = FindObjectsByType<PlayerMouvement>(FindObjectsSortMode.None);
        Debug.Log("movements count == " + movements.Length);
        return movements.Length;
    }
}

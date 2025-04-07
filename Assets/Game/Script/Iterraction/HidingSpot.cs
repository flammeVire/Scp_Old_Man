using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class HidingSpot : NetworkBehaviour
{
    public NetworkBool IsSomeoneHide;
    public Camera HidingCamera;
    public Transform Inside;
    public Transform Outside;
    GameObject playerInside;
    public void HidePlayer(PlayerMouvement player)
    {
        player.GetComponentInChildren<Camera>().enabled = false;
        HidingCamera.enabled = true;
        Rpc_HidePlayerMesh(player);
        StartCoroutine(WaitForPlayerExit());
    }

    [Rpc(RpcSources.All,RpcTargets.All)]
    public void Rpc_HidePlayerMesh(PlayerMouvement player)
    {
        playerInside = player.gameObject;
        playerInside.SetActive(false);
        IsSomeoneHide = true;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_ShowPlayerMesh()
    {
        playerInside.SetActive(true);
        playerInside.GetComponent<PlayerMouvement>().TeleportMesh(Outside.position, Outside.rotation);
        if ((playerInside.GetComponent<NetworkObject>().HasInputAuthority))
        {
            HidingCamera.enabled = false;
            playerInside.GetComponentInChildren<Camera>().enabled = true;
        }
        playerInside = null;
        IsSomeoneHide = false;
    }

    IEnumerator WaitForPlayerExit()
    {
        yield return new WaitForSeconds(0.5f);
        Debug.Log("Input");
        yield return new WaitUntil(() => Input.GetButtonDown("Interract"));
        Rpc_ShowPlayerMesh();
    }
    public bool IsSomeoneInside()
    {
        return true;
    }
}

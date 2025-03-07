using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : NetworkBehaviour, IPlayerJoined
{
    public GameObject PlayerUIName;
    public GameObject PlayerUIParent;
    public static GameManager instance;
    [Networked] public NetworkDictionary<PlayerRef,PlayerManager> PlayerRefDict => default;
    
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    [Rpc(RpcSources.All,RpcTargets.All)]
    public void Rpc_ReferencePlayer(PlayerRef player,PlayerManager playermanager)
    {
        Debug.Log(player + "is add to dictionary");
        if (!PlayerRefDict.ContainsKey(player))
        {
            PlayerRefDict.Add(player, playermanager);
            UpdateUIForLobby();
        }
    }
    public void UpdateUIForLobby()
    {
        Debug.Log("Update UI");
        GameObject obj = Instantiate(PlayerUIName, PlayerUIParent.transform.position, Quaternion.identity);
        obj.transform.parent = PlayerUIParent.transform;
    }
    public void PlayerJoined(PlayerRef player)
    {
        Debug.Log("new player joined " + player);
    }

    public void GetOtherPlayer()
    {
       /// Runner.ActivePlayers
    }
}

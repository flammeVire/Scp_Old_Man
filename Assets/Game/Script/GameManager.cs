using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour, IPlayerJoined 
{
    [SerializeField]GameObject GlobalCanva;
    public GameObject PlayerUIName;
    public GameObject PlayerUIParent;
    public static GameManager instance;
    //[Networked] 
    public List<NetworkRunner> Runners = new List<NetworkRunner>();
    [SerializeField]GameObject StartButton;
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    /*
     * quand Je créé le lobby -> je suis le joueur 1 et personne d'autre
     * quand j'arrive je repertori les autre joueurs au dico
     * quand un joueur viens on l'ajoute au dico
     */


    public void PlayerJoined(PlayerRef player)
    {
        GetFirstPlayer();
    }

    [Rpc(RpcSources.All,RpcTargets.All)]
    public void Rpc_AddingPlayerToList(PlayerManager manager)
    {
        if (!Runners.Contains(manager.runner))
        {
            Runners.Add(manager.runner);
        }
    }

    public void UpdatePlayersList()
    {
        
        foreach (NetworkRunner runner in Runners) 
        {
            Debug.Log("spawn ui");
            GameObject ui = Instantiate(PlayerUIName, PlayerUIParent.transform.position,PlayerUIParent.transform.rotation,PlayerUIParent.transform);
            ui.GetComponentInChildren<TextMeshProUGUI>().text = runner.LocalPlayer.ToString();
        }
    }

    void GetFirstPlayer()
    {
        int numberOfPlayers = new List<PlayerRef>(Runner.ActivePlayers).Count;
        Debug.Log("there is " + numberOfPlayers + " player in game");
        if (numberOfPlayers == 1)
        {
            GameObject clone = Instantiate(StartButton, GlobalCanva.transform.position, GlobalCanva.transform.rotation, GlobalCanva.transform);
        }
    }

    public void LaunchGame()
    {
        Debug.Log("GameLaunched");
    }

    /*
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
*/
}

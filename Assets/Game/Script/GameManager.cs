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

    /*
     * quand Je créé le lobby -> je suis le joueur 1 et personne d'autre
     * quand j'arrive je repertori les autre joueurs au dico
     * quand un joueur viens on l'ajoute au dico
     */


    public void PlayerJoined(PlayerRef player)
    {

    }

    public void getOtherPlayer()
    {
        List<PlayerRef> oldestPlayers = new List<PlayerRef>();
        foreach(var players in Runner.ActivePlayers)
        {
            oldestPlayers.Add(players);
        }
        //recuperer le playerManager Lié
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

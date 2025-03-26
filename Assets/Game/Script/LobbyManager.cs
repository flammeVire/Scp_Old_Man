using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : NetworkBehaviour, IPlayerJoined
{
    public static LobbyManager instance;
    [SerializeField]GameObject GlobalCanva;
    public GameObject PlayerUIName;
    public GameObject PlayerUIParent;
    public GameObject PlayerPrefab;
    public SceneRef sceneRef;
    public PlayerRef FirstPlayer;

    public List<PlayerManager> playerList = new List<PlayerManager>();
    [SerializeField]GameObject StartButton;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void IPlayerJoined.PlayerJoined(PlayerRef player)
    {
        Debug.Log("---- player joined " + player + " ----"); //s'affiche pour le 1er joueur
        SpawnPlayerManager(player);
        if(FirstPlayer == PlayerRef.None)
        {
            Debug.Log("Set First Player to " + player);
            FirstPlayer = player;
            SpawnLaunchButton();
        }
        else
        {
            Debug.Log("First player already set to " + FirstPlayer);
        }
    }

    void SpawnPlayerManager(PlayerRef player)
    {
        Debug.Log("Spawn playerManager and add " + player + "To authority");
        NetworkSpawnOp obj = Runner.SpawnAsync(PlayerPrefab,Vector3.zero,Quaternion.identity,player);
        AddNewPlayerToList(obj.Object.GetComponent<PlayerManager>());
    }
    void AddNewPlayerToList(PlayerManager manager)
    {
        if (!playerList.Contains(manager))
        {
            Debug.Log("Adding Manager");
            playerList.Add(manager);
        }
    }

    void SpawnLaunchButton() // only for 1st player
    {
        Debug.Log("SpawnLaunchButton");
        GameObject clone = Instantiate(StartButton, GlobalCanva.transform.position, GlobalCanva.transform.rotation, GlobalCanva.transform);
        clone.GetComponentInChildren<Button>().onClick.AddListener(Rpc_LaunchGame);
    }

    void Rpc_LaunchGame()
    {
        ///Summary: charge la scene pour tout les joueurs
        Runner.LoadScene(sceneRef, loadSceneMode: UnityEngine.SceneManagement.LoadSceneMode.Single);
        DontDestroyOnLoad(gameObject);
        Debug.Log("LoadScene");
    }
    /// recuperer tout les managers pour chaque joueur
    /// s'assuré qu'il n'y a pas de doublons
    /// s'assuré que le 1er joueur est le 1er joueur

    /*
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
    public void PlayerJoined(PlayerRef player)
    {
        Debug.Log("----  PlayerJoined " + player + "  ----");
        SpawnPlayer(player);
        if (true) 
        {

        }
    }
    public void SpawnPlayer(PlayerRef player)
    {
        Debug.Log(player);
        if (NetworkManager.runnerInstance.IsServer)
        {
            Debug.Log("playerJoin " + player + "and prefab spawn");
            NetworkObject playerObject = NetworkManager.runnerInstance.Spawn(PlayerPrefab, Vector3.up, Quaternion.identity,player);
            NetworkManager.runnerInstance.SetPlayerObject(player, playerObject);
            playerObject.GetComponent<PlayerManager>().playerRef = player;
            
        }
    }

    [Rpc(RpcSources.All,RpcTargets.All)]
    public void Rpc_AddingPlayerToList(PlayerManager manager)
    {
        Debug.Log(Runner);
        if (!playerList.Contains(manager))
        {

            playerList.Add(manager);
            Debug.Log("adding manager");
        }
        else
        {
            Debug.Log("manager already exist");
        }
    }

    public void UpdatePlayersList()
    {
        
        foreach (PlayerManager manager in playerList) 
        {
            Debug.Log("spawn ui");
            GameObject ui = Instantiate(PlayerUIName, PlayerUIParent.transform.position,PlayerUIParent.transform.rotation,PlayerUIParent.transform);
           // ui.GetComponentInChildren<TextMeshProUGUI>().text = runner.LocalPlayer.ToString();
        }
    }

    public void AddLaunchButton()
    {
        if (FirstPlayer == null)
        {
            GameObject clone = Instantiate(StartButton, GlobalCanva.transform.position, GlobalCanva.transform.rotation, GlobalCanva.transform);
            clone.GetComponentInChildren<Button>().onClick.AddListener(Rpc_LaunchGame);
        }
    }

   // [Rpc(RpcSources.StateAuthority,RpcTargets.All)]
    public void Rpc_LaunchGame()
    {
        ///Summary: charge la scene pour tout les joueurs
        Runner.LoadScene(sceneRef, loadSceneMode: UnityEngine.SceneManagement.LoadSceneMode.Single);
        DontDestroyOnLoad(gameObject);
        Debug.Log("LoadScene");
    }

    */


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

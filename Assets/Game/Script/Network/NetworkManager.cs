using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;
using UnityEngine.SceneManagement;
using TMPro;




public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
    public static NetworkRunner runnerInstance;
    public string lobbyName = "default";

    public Transform sessionListContentParent;
    public GameObject CreateGame;
    public GameObject lobby_Session_Prefab;
    public Dictionary<string, GameObject> SessionListUIDictionary = new Dictionary<string, GameObject>();

    public string PlayScene;
    public string LobbyScene;

    public GameObject PlayerPrefab;

    #region Unity Function
    private void Awake()
    {
        runnerInstance = gameObject.GetComponent<NetworkRunner>();

        if (runnerInstance == null)
        {
            runnerInstance = gameObject.AddComponent<NetworkRunner>();
        }
    }

    private void Start()
    {
        
        runnerInstance.JoinSessionLobby(SessionLobby.Shared, lobbyName);
    }

    #endregion

    #region lobby button
    public static void ReturnToLobby()
    {
        if (NetworkManager.runnerInstance.GetPlayerObject(runnerInstance.LocalPlayer) != null)
        {
            NetworkManager.runnerInstance.Despawn(runnerInstance.GetPlayerObject(runnerInstance.LocalPlayer));
        }
        NetworkManager.runnerInstance.Shutdown(true, ShutdownReason.Ok);
    }

    public void CreateRandomSession()
    {
        int randomInt = UnityEngine.Random.Range(1000, 9999);

        string randomSessionName = "Room-" + randomInt.ToString();
        string PlaySceneName = PlayScene;
        Scene scene = SceneManager.GetSceneByName(PlaySceneName);
        int sceneIndex = scene.buildIndex;
        runnerInstance.StartGame(new StartGameArgs()
        {
            Scene = SceneRef.FromIndex(GetSceneIndex(PlayScene)),
            SessionName = randomSessionName,
            GameMode = GameMode.Shared,
            PlayerCount = 4
        });
    }

    public int GetSceneIndex(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            String name = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            if (name == sceneName)
            {
                return i;
            }
        }
        return -1;
    }
    #endregion

    #region INetworkRunnerCallBacks Function Used

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        
    }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        DeleteOldSessionsFromUI(sessionList);
        CompareLists(sessionList);
        StartCoroutine(WaitUntilConnected());
    }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        SceneManager.LoadScene(LobbyScene);
    }
    #endregion

    #region Session ui
    private void CompareLists(List<SessionInfo> sessionList)
    {
        foreach (SessionInfo session in sessionList)
        {
            if (SessionListUIDictionary.ContainsKey(session.Name))
            {
                UpdateEntryUI(session);
            }
            else
            {
                CreateEntryUI(session);
            }
        }
    }

    private void CreateEntryUI(SessionInfo session)
    {
        GameObject newEntry = Instantiate(lobby_Session_Prefab);
        newEntry.transform.parent = sessionListContentParent;
        SessionListEntry entryScript = newEntry.GetComponent<SessionListEntry>();
        SessionListUIDictionary.Add(session.Name, newEntry);
        entryScript.roomName.text = session.Name;
        entryScript.PlayerCount.text = session.PlayerCount.ToString() + "/" + session.MaxPlayers.ToString();
        entryScript.JoinButton.interactable = session.IsOpen;

        newEntry.SetActive(session.IsVisible);
    }

    private void UpdateEntryUI(SessionInfo session)
    {
        SessionListUIDictionary.TryGetValue(session.Name, out GameObject newEntry);

        SessionListEntry entryScript = newEntry.GetComponent<SessionListEntry>();

        entryScript.roomName.text = session.Name;
        entryScript.PlayerCount.text = session.PlayerCount.ToString() + "/" + session.MaxPlayers.ToString();
        entryScript.JoinButton.interactable = session.IsOpen;

        newEntry.SetActive(session.IsVisible);
    }
    private void DeleteOldSessionsFromUI(List<SessionInfo> sessionList)
    {
        bool isContained = false;
        GameObject uiToDelete = null;

        foreach (KeyValuePair<string, GameObject> kvp in SessionListUIDictionary)
        {
            string sessionkey = kvp.Key;
            foreach (SessionInfo session in sessionList)
            {
                if (session.Name == sessionkey)
                {
                    isContained = true;
                    break;
                }
            }
            if (!isContained)
            {
                uiToDelete = kvp.Value;
                SessionListUIDictionary.Remove(sessionkey);
                Destroy(uiToDelete);
            }
        }
    }
    #endregion

    #region INetworkRunnerCallBacks Function not Used
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }


    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    #endregion

    IEnumerator WaitUntilConnected()
    {
        yield return new WaitUntil(() => this.GetComponent<NetworkRunner>().IsCloudReady);

       
        CreateGame.SetActive(true);
        Debug.Log("Connected");
    }

}
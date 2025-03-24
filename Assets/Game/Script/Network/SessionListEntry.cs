using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Fusion;
using UnityEngine.UI;
using Photon.Realtime;


public class SessionListEntry : MonoBehaviour
{
    public TextMeshProUGUI roomName, PlayerCount;
    public Button JoinButton;

    public void JoinRoom()
    {
        NetworkManager.runnerInstance.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Client,
            SessionName = roomName.text,

        });
        
        Debug.Log(roomName.text);
    }
}

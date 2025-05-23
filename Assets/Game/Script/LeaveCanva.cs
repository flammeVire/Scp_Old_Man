using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
public class LeaveCanva : MonoBehaviour
{
    public GameObject LeavePanel;

    void Update()
    {
            if (Input.GetButtonDown("Cancel"))
            {
                if (Input.GetButtonDown("Cancel") && LeavePanel.activeInHierarchy)
                {
                    LeavePanel.SetActive(false);
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
                else if (Input.GetButtonDown("Cancel") && !LeavePanel.activeInHierarchy)
                {
                    LeavePanel.SetActive(true);
                    Cursor.lockState = CursorLockMode.Confined;
                    Cursor.visible = true;
                }
            }
        
    }

    public void Resume()
    {
        Debug.Log("Resume");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        LeavePanel.SetActive(false);
    }

    public void Leave()
    {
        Debug.Log("Return to lobby");
        NetworkManager.ReturnToLobby();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityCam : MonoBehaviour
{
    
    public Camera playerCamera;

    public Camera[] AllCamera;
    int index = 0;

    public void PowerOn()
    {
        ManageMovement(false);
        playerCamera.enabled = false;
        AllCamera[index].enabled = true;
        
    }
    public void PowerOff()
    {
        foreach (var camera in AllCamera) 
        {
            if (camera.enabled)
            {
                camera.enabled = false;
            }
        }
        playerCamera.enabled = true;
        ManageMovement(true);
        this.enabled = false;
    }

    public void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            SwitchCam();
        }
        else if (Input.GetButtonDown("Fire1"))
        {
            PowerOff();
        }
    }

    void SwitchCam()
    {
        index++;
        if (index >= AllCamera.Length) 
        {
            index = 0;
        }
        for (int i = 0; i < AllCamera.Length; i++) 
        {
            if(i == index)
            {
                AllCamera[i].enabled = true;
            }
            else
            {
                AllCamera[i].enabled = false;
            }
        }
    }

    public void ManageMovement(bool state)
    {
        playerCamera.GetComponentInParent<PlayerMouvement>().enabled = state;
        playerCamera.GetComponentInParent<PickItem>().enabled = state;
    }
}

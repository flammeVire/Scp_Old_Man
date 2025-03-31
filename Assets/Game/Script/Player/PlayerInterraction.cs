using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInterraction : NetworkBehaviour
{
    public Camera playerCamera;
    void Update()
    {
        InputManagement();
    }

    void InputManagement()
    {
        if (HasInputAuthority)
        {
            if (Input.GetButtonDown("Interract"))
            {
                Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 2f))
                {
                    if (hit.collider.CompareTag("Interractive"))
                    {
                        if (hit.collider.GetComponent<SecurityCam>())
                        {
                            SecurityCam(hit.collider.GetComponent<SecurityCam>());

                        }
                        if (hit.collider.GetComponent<Card_Door>())
                        {
                            Debug.Log("Hit Door");
                            hit.collider.GetComponent<Card_Door>().Interract(this.GetComponent<PickItem>());
                        }
                    }
                }
            }
        }
    }

    void SecurityCam(SecurityCam security)
    {
        
        security.enabled = true;
        security.playerCamera = playerCamera;
        security.PowerOn();
    }
}

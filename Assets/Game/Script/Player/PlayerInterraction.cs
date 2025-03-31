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
                        else if (hit.collider.GetComponent<Card_Door>())
                        {
                            Debug.Log("Hit Door");
                            hit.collider.GetComponent<Card_Door>().Interract(this.GetComponent<PickItem>());
                        }
                        else if (hit.collider.GetComponent<ButtonDouble_Door>()) 
                        {
                            Debug.Log("Hit button double Door");
                            ButtonDoubleDoor(hit.collider.GetComponent<ButtonDouble_Door>());
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

    void ButtonDoubleDoor(ButtonDouble_Door button)
    {
        button.ActiveButton(true);
        StartCoroutine(WaitUntilInterractIsUp(button));
    }
    IEnumerator WaitUntilInterractIsUp(ButtonDouble_Door button)
    {
        yield return new WaitUntil(() => Input.GetButtonUp("Interract"));
        Debug.Log("Interract is up");
        yield return new WaitForSecondsRealtime(5);
        button.ActiveButton(false);
    }
}

using Fusion;
using System.Collections;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class PlayerInterraction : NetworkBehaviour
{
    public Camera playerCamera;
    public StudioEventEmitter hide;
    public PlayerMouvement mouvement;
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
                        //MattSounds : jouer son interraction (tu peux descendre dans les if selons les son)
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
                        else if (hit.collider.GetComponent<Sas_Door>())
                        {
                            Sas_Door(hit.collider.GetComponent<Sas_Door>());
                        }
                        else if (hit.collider.GetComponent<EndGame>())
                        {
                            EndGame(hit.collider.GetComponent<EndGame>());
                        }
                    }
                    else if (hit.collider.CompareTag("Hide"))
                    {
                        //MattSounds (jouer son se cacher (pas encore mis de cachette ingame)
                        //hide.Play();
                        Hide(hit.collider.GetComponent<HidingSpot>());
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
        button.Rpc_ActiveButton(true);
        StartCoroutine(WaitUntilInterractIsUp(button));
    }
    IEnumerator WaitUntilInterractIsUp(ButtonDouble_Door button)
    {
        yield return new WaitUntil(() => Input.GetButtonUp("Interract"));
        yield return new WaitForSecondsRealtime(5);
        Debug.Log("Interract is up");
        button.Rpc_ActiveButton(false);
    }
    void Sas_Door(Sas_Door sas)
    {
        if (!sas.anim.isPlaying)
        {
            mouvement.CanMove = false;

            sas.Rpc_OpeningSas(mouvement);
            StartCoroutine(MoveDelay(sas.Delay));
        }
    }

    IEnumerator MoveDelay(float delay)
    {
        Debug.Log("Wait for " + delay);
        yield return new WaitForSeconds(delay);
        mouvement.CanMove = true;
        Debug.Log("CanMove == " + mouvement.CanMove);
    }

    void EndGame(EndGame endGame)
    {
        Debug.Log("EndGame");
        endGame.Rpc_ClosingEndSas(true);
        StartCoroutine(WaitUntilInterractIsUp(endGame));
    }
    IEnumerator WaitUntilInterractIsUp(EndGame endGame)
    {
        yield return new WaitUntil(() => Input.GetButtonUp("Interract"));
        yield return new WaitForSecondsRealtime(endGame.Delay);
        Debug.Log("Interract is up");
        endGame.Rpc_ClosingEndSas(false);
    }
    void Hide(HidingSpot hidingSpot)
    {
        if (!hidingSpot.IsSomeoneHide)
        {
            hidingSpot.HidePlayer(this.GetComponent<PlayerMouvement>());
        }
        
    }
}

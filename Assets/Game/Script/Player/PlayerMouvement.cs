using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Photon.Realtime;

public class PlayerMouvement : NetworkBehaviour
{
    public float Speed;
    public float JumpHeight;
    public float sensitivity = 2f;
    public float rotationX = 0f;
    public Rigidbody body;
    public GameObject cam;



    public void Start()
    {
        if (HasInputAuthority)
        {
            GetComponentInChildren<Camera>().enabled = true;
            Debug.Log("Camera.enable == " + GetComponentInChildren<Camera>().enabled);
        }
        else
        {
            Debug.Log("nan tg");
        }
    }



    public override void FixedUpdateNetwork()
    {
        if (HasInputAuthority)
        {
            Debug.Log($" {gameObject.name} has InputAuthority and is trying to move.");

            if (cam == null)
            {
                Debug.LogError($"[{Time.time}] {gameObject.name} cam is NULL!");
            }

            Rotate();
            Movement();
        }
        else
        {
            Debug.Log($" {gameObject.name} has NO input authority.");
        }
    }

    void Movement()
    {
        // Récupération des entrées clavier
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");


        Debug.Log(horizontal + " " + vertical);
        // Direction du mouvement selon l'orientation de la caméra
        Debug.Log("cam = " + cam);

        Vector3 direction = cam.transform.forward * vertical + cam.transform.right * horizontal;
        Debug.Log("Direction == " + direction);
        direction.y = 0; // On empêche le mouvement vertical

        // Appliquer la vélocité
        body.velocity = direction.normalized * Speed;

    }

    void Rotate()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        // Rotation horizontale du joueur
        transform.Rotate(Vector3.up * mouseX);

        // Rotation verticale de la caméra
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f); // Empêche la caméra de se retourner totalement
        cam.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerManager : NetworkBehaviour
{
    private Rigidbody body;

    public float Speed;
    public Transform cam; // Référence à la caméra pour orienter les déplacements
    public float sensitivity = 2f; // Sensibilité de la souris

    private float rotationX = 0f; // Rotation verticale de la caméra


    private void Start()
    {
        if (HasStateAuthority)
        {
            body = GetComponent<Rigidbody>();
            body.freezeRotation = true;
            cam = GetComponentInChildren<Camera>().transform;
            cam.GetComponent<Camera>().enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (HasStateAuthority) //si joueur controlle le script (evite de bouger les autres joueurs)
        {
            Movement();
            Rotate();
        }
    }

    void Movement()
    {
        // Récupération des entrées clavier
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // Direction du mouvement selon l'orientation de la caméra
        Vector3 direction = cam.forward * vertical + cam.right * horizontal;
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
        cam.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
    }
}

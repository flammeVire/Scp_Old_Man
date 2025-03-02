using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerManager : NetworkBehaviour
{
    private Rigidbody body;

    public float Speed;
    public Transform cam; // R�f�rence � la cam�ra pour orienter les d�placements
    public float sensitivity = 2f; // Sensibilit� de la souris

    private float rotationX = 0f; // Rotation verticale de la cam�ra


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
        // R�cup�ration des entr�es clavier
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // Direction du mouvement selon l'orientation de la cam�ra
        Vector3 direction = cam.forward * vertical + cam.right * horizontal;
        direction.y = 0; // On emp�che le mouvement vertical

        // Appliquer la v�locit�
        body.velocity = direction.normalized * Speed;
    }

    void Rotate()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        // Rotation horizontale du joueur
        transform.Rotate(Vector3.up * mouseX);

        // Rotation verticale de la cam�ra
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f); // Emp�che la cam�ra de se retourner totalement
        cam.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
    }
}

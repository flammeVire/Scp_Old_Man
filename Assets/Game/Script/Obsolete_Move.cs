using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    public Transform cam; // R�f�rence � la cam�ra pour orienter les d�placements
    public float moveSpeed = 3f; // Vitesse de d�placement
    public float sensitivity = 2f; // Sensibilit� de la souris

    private Rigidbody body;
    private float rotationX = 0f; // Rotation verticale de la cam�ra

    void Start()
    {
        body = GetComponent<Rigidbody>();
        body.freezeRotation = true; // Emp�che le Rigidbody de tourner � cause de collisions

        // Verrouillage du curseur au centre de l'�cran
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Movement();
        Rotate();
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
        body.velocity = direction.normalized * moveSpeed;
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

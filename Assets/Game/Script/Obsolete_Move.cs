using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    public Transform cam; // Référence à la caméra pour orienter les déplacements
    public float moveSpeed = 3f; // Vitesse de déplacement
    public float sensitivity = 2f; // Sensibilité de la souris

    private Rigidbody body;
    private float rotationX = 0f; // Rotation verticale de la caméra

    void Start()
    {
        body = GetComponent<Rigidbody>();
        body.freezeRotation = true; // Empêche le Rigidbody de tourner à cause de collisions

        // Verrouillage du curseur au centre de l'écran
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
        // Récupération des entrées clavier
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // Direction du mouvement selon l'orientation de la caméra
        Vector3 direction = cam.forward * vertical + cam.right * horizontal;
        direction.y = 0; // On empêche le mouvement vertical

        // Appliquer la vélocité
        body.velocity = direction.normalized * moveSpeed;
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

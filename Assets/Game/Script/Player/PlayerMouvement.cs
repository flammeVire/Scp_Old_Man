using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerMouvement : NetworkBehaviour
{
    public float Speed;
    public float JumpHeight;
    public float sensitivity = 2f;
    public float rotationX = 0f;
    public Rigidbody body;
    [HideInInspector]public GameObject cam;
    [SerializeField] GameObject camPrefab;

    public void Start()
    {
        if (HasInputAuthority)
        {
            GameObject clone = Instantiate(camPrefab);
            clone.transform.parent = transform;
            cam = clone;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void Update()
    {
        if (HasInputAuthority) //si le joueur a le controle des input => evite de faire bouger autre joueur
        {
            GetPlayerInput();
        }
    }

    public void GetPlayerInput()
    {
        // Récupération des entrées clavier
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (cam != null)
        {
            Vector3 direction = cam.transform.forward * vertical + cam.transform.right * horizontal;
            direction.y = 0; // Empêcher le mouvement vertical

            // Appliquer la vélocité
            body.velocity = direction.normalized * Speed;

            // Récupération des entrées souris
            float mouseX = Input.GetAxis("Mouse X") * sensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

            // Debug pour vérifier les entrées
            Debug.Log("Mouse X: " + mouseX + " | Mouse Y: " + mouseY);

            //  Rotation horizontale du joueur (Gauche/Droite)
            transform.Rotate(0f, mouseX, 0f); // Rotation en Y

            //  Rotation verticale de la caméra (Haut/Bas)
            rotationX -= mouseY;
            rotationX = Mathf.Clamp(rotationX, -90f, 90f); // Empêche de retourner la tête

            cam.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);

            // Debug pour voir la rotation de la caméra
            Debug.Log("Cam Rotation: " + cam.transform.localRotation.eulerAngles);
        }
    }

}

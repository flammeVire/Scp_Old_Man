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
            GameObject obj = Instantiate(camPrefab, new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z + 0.2f), Quaternion.identity);
            cam = obj;
            obj.transform.parent = this.transform;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (HasInputAuthority) //si le joueur a le controle des input => evite de faire bouger autre joueur
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
            if (cam != null)
            {
                Vector3 direction = cam.transform.forward * vertical + cam.transform.right * horizontal;
                direction.y = 0; // On empêche le mouvement vertical

                // Appliquer la vélocité
                body.velocity = direction.normalized * Speed;
            }
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

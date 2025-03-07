using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerManager : NetworkBehaviour
{
    private Rigidbody body;
    public PlayerRef playerRef;
    public GameManager gameManager;
    public float Speed;
    public GameObject cam; // R�f�rence � la cam�ra pour orienter les d�placements
    public float sensitivity = 2f; // Sensibilit� de la souris

    private float rotationX = 0f; // Rotation verticale de la cam�ra


    private void Start()
    {
        Debug.Log("this == " + this);
        Debug.Log("playerRef == " + Runner.LocalPlayer);
        Debug.Log("gameManager =" + gameManager);
        playerRef = Runner.LocalPlayer;
        if (HasStateAuthority)
        {
            body = GetComponent<Rigidbody>();
            body.freezeRotation = true;
            GameObject obj = Instantiate(cam, new Vector3(transform.position.x,transform.position.y+0.5f,transform.position.z+0.2f), Quaternion.identity);
            obj.transform.parent = this.transform;
            
            cam = obj;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        GameManager.instance.Rpc_ReferencePlayer(Runner.LocalPlayer,this);
        GameManager.instance.GetOtherPlayer();
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
        if (cam != null)
        {
            Vector3 direction = cam.transform.forward * vertical + cam.transform.right * horizontal;
            direction.y = 0; // On emp�che le mouvement vertical

            // Appliquer la v�locit�
            body.velocity = direction.normalized * Speed;
        }
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
        cam.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
    }
}

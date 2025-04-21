using Fusion;
using System.Linq;
using UnityEngine;

public class PlayerMouvement : NetworkBehaviour, ISpawned
{
    public Transform cam;
    public float moveSpeed = 3f;
    public float sensitivity = 2f;
    public float jumpForce = 5f;
    public float gravity = 9.81f;
    public float crouchHeight = 0.5f;
    public float standingHeight = 2f;
    public float stamina = 100;
    private Rigidbody body;
    private float rotationX = 0f;
    public bool isGrounded;
    public bool isCrouching;
    public bool isRunning;
    public bool isMoving;
    public bool isTalking;
    private CapsuleCollider playerCollider;
    void Start()
    {
        if (HasInputAuthority)
        {
            body = GetComponent<Rigidbody>();
            body.freezeRotation = true;
            playerCollider = GetComponent<CapsuleCollider>();

            cam.GetComponent<Camera>().enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Update()
    {
        if (HasInputAuthority)
        {
            Movement();
            Rotate();
            HandleJump();
            HandleCrouch();
            HandleRun();
            if (Input.GetKeyDown(KeyCode.P))
            {
                Vector3 pos = new Vector3(transform.position.x, transform.position.y - 8, transform.position.z);
                Rpc_TeleportMesh(pos, transform.rotation);
            }
            if (Input.GetKeyDown(KeyCode.M))
            {
                Vector3 pos = new Vector3(transform.position.x, transform.position.y + 12, transform.position.z);
                Rpc_TeleportMesh(pos, transform.rotation);
            }
            
        }
    }

    void Movement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 direction = cam.forward * vertical + cam.right * horizontal;
        direction.y = 0;

        body.velocity = new Vector3(direction.normalized.x * moveSpeed, body.velocity.y, direction.normalized.z * moveSpeed);
        if (horizontal != 0 || vertical != 0) 
        {
            isMoving = true;
        }
        else if(horizontal == 0 && vertical == 0)
        {
            isMoving = false;
        }
    }

    void Rotate()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        transform.Rotate(Vector3.up * mouseX);

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -89f, 89f);
        cam.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
    }

    void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Debug.Log("Jump");
            body.velocity = new Vector3(body.velocity.x, jumpForce, body.velocity.z);
        }

    }

    void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (!isCrouching)
            {
                playerCollider.height = crouchHeight;
                moveSpeed /= 2;
                isCrouching = true;
            }
            else
            {
                playerCollider.height = standingHeight;
                moveSpeed *= 2;
                isCrouching = false;
            }
        }
    }

    void HandleRun()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            if (!isRunning)
            {
                moveSpeed *= 2;
                isRunning = true;
            }
            else
            {
                moveSpeed /= 2;
                isRunning = false;
            }
        }
    }

    void FixedUpdate()
    {
        if (HasInputAuthority)
        {
            isGrounded = Physics.Raycast(transform.position, Vector3.down, playerCollider.bounds.extents.y + 0.1f);
            if (!isGrounded)
            {
                body.velocity += Vector3.down * gravity * Time.fixedDeltaTime;
            }
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_TeleportMesh(Vector3 spawnPosition, Quaternion rotation)
    {
        transform.position = spawnPosition;
        transform.rotation = rotation;
    }

    public override void Spawned()
    {
        Init();
    }

    
    public void Init()
    {
        Debug.Log("Init " + NetworkManager.runnerInstance.LocalPlayer);
        if (GameManager.Instance.NumberOfMesh() == NetworkManager.runnerInstance.ActivePlayers.Count())
        {
            Debug.Log("All Mesh Are Here");
            GameManager.Instance.Rpc_GetAllMeshes();
        }
    }
}

using Fusion;
using UnityEngine;
using System.Collections;
public class PlayerMouvement : NetworkBehaviour
{
    public Transform cam;
    public float moveSpeed = 3f;
    public float normalSpeed = 3f;
    public float sprintSpeed = 6f;
    public float crouchSpeed = 1.5f;
    public float sensitivity = 2f;
    public float jumpForce = 5f;
    public float gravity = 9.81f;
    public float crouchHeight = 0.5f;
    public float standingHeight = 2f;
    public float stamina = 100;
    private Rigidbody body;
    private float rotationX = 0f;
    private bool isGrounded;
    private bool isCrouching;
    private bool isRunning;
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
            if (Input.GetButtonDown("P"))
            {
                Vector3 pos = new Vector3(transform.position.x, transform.position.y-8,transform.position.z);
                TeleportMesh(pos, transform.rotation);
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
        if (!isCrouching && !isRunning && Input.GetKeyDown(KeyCode.Y) && stamina > 10)
        {
            stamina -= 0.1f;
            moveSpeed = sprintSpeed;
            isRunning = true;
        }

        else if (!isCrouching && isRunning && Input.GetKey(KeyCode.Y) && stamina > 0)
        {
            stamina -= 0.1f;
        }
        else if ( isRunning && Input.GetKeyUp(KeyCode.Y) || isRunning && stamina <= 0)
        {
            moveSpeed = normalSpeed;
            stamina += 0.1f;
            isRunning = false;
        }
        else
        {
            if (stamina < 100)
            {
                stamina += 0.1f;
            }
        }
        Debug.Log("Stamina = " + stamina);
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

    public void TeleportMesh(Vector3 spawnPosition, Quaternion rotation)
    {
        transform.position = spawnPosition;
        transform.rotation = rotation;
    }
}

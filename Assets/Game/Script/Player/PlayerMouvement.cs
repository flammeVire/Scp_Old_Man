using Fusion;
using Photon.Voice.Unity;
using System.Collections;
using System.Linq;
using UnityEngine;

public class PlayerMouvement : NetworkBehaviour, ISpawned
{
    #region Data
    public Transform cam;
    [Header("Speed",order = 0)]
    public float Speed;
    public float moveSpeed = 3f;
    public float CrouchSpeed = 1f;
    public float RunSpeed = 5f;

    [Header("ImportantData")]
    public float sensitivity = 2f;
    public float jumpForce = 5f;
    public float crouchHeight = 0.5f;
    public float standingHeight = 2f;
    public float stamina = 100;
    float decreaseStamina = 0.5f;
    float increaseStamina = 0.1f;

    private CapsuleCollider playerCollider;
    [HideInInspector]public float gravity = 9.81f;
    private float rotationX = 0f;
    private Rigidbody body;
    
    [Header("Boolean")]
    public bool isGrounded;
    public bool isCrouching;
    public bool isJumping;
    public bool isRunning;
    public bool isMoving;
    public bool isTalking;

    [Header("Slow Corrosion")]
    private float corrosionSlow = 0.5f;
    private bool inCorrosionZone = false;

    #endregion
    #region Unity default Function
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
    void Update()
    {
        if (HasInputAuthority)
        {
            Talk();
            Movement();
            Rotate();
            HandleJump();
            HandleCrouch();
            HandleRun();
            ManageStamina(isRunning);
            ManageSpeed();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (HasInputAuthority)
        {
            if (other.CompareTag("Corrosion"))
            {
                inCorrosionZone = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (HasInputAuthority)
        {
            if (other.CompareTag("Corrosion"))
            {
                inCorrosionZone = false;
            }
        }
    }
    #endregion
    #region AllMovement
    void Movement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 direction = cam.forward * vertical + cam.right * horizontal;
        direction.y = 0;
        body.velocity = new Vector3(direction.normalized.x * Speed, body.velocity.y, direction.normalized.z * Speed);

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
            body.velocity = new Vector3(body.velocity.x, jumpForce, body.velocity.z);
            isJumping = true;
            playerCollider.height = standingHeight;
            isCrouching = false;
            StartCoroutine(JumpDetection());
        }

    }

    void HandleCrouch()
    {
        if (Input.GetButtonDown("Crouch"))
        {
            if (!isCrouching)
            {  
                isCrouching = true;
            }
            else
            {
                isCrouching = false;
            }
        }

        if (isCrouching)
        {
            playerCollider.height = crouchHeight;
        }
        else
        {
            playerCollider.height = standingHeight;
        }
    }

    void HandleRun()
    {
        if (!isCrouching)
        {
            if (Input.GetButtonDown("Run"))
            {
                Debug.Log("Running");
                isRunning = true;
            }
            else if (Input.GetButtonUp("Run") || stamina <= 0)
            {
                Debug.Log("Not Running");
                isRunning = false;
            }
        }
        else
        {
            isRunning = false;
        }
    }
    

    #endregion

    void ManageStamina(bool IsRunning)
    {
        
        //case: stamina increase
        if(!IsRunning)
        {
            if (stamina > 100) 
            {
                stamina = 100;
            }
            else
            {
                stamina += increaseStamina;
            }
        }


            //case: stamina decrease
        else if(IsRunning)
        {
            if(stamina < 0)
            {
                stamina = 0;
            }
            else
            {
                stamina -= decreaseStamina;
            }
        }
    }

    public void ManageSpeed()
    {
        if (isCrouching)
        {
            Speed = CrouchSpeed;
        }
        else if (isRunning)
        {
            Speed = RunSpeed;
        }
        else
        {
            Speed = moveSpeed;
        }
        if (inCorrosionZone)
        {
            Speed *= corrosionSlow;
        }
    }

   

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_TeleportMesh(Vector3 spawnPosition, Quaternion rotation)
    {
        transform.position = spawnPosition;
        transform.rotation = rotation;
    }

    public void Talk()
    {
        if (Input.GetButtonDown("Talk"))
        {
            FindAnyObjectByType<Recorder>().RecordingEnabled = true;
            isTalking = true;
            Debug.Log("Talk");
        }
        else if (Input.GetButtonUp("Talk"))
        {
            FindAnyObjectByType<Recorder>().RecordingEnabled = false;
            isTalking = false;
            Debug.Log("UnTalk");
        }
    }

    IEnumerator JumpDetection()
    {
        yield return new WaitForSeconds(0.2f);
        isJumping = false;
    }
    #region SpawnManagement
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
        if (HasInputAuthority)
        {
            InstantiateUI();
        }
    }

    public void InstantiateUI()
    {
        GameObject ui = Instantiate(GameManager.Instance.PlayerUI, Vector3.zero, Quaternion.identity);
        ui.GetComponent<PlayerUI>().mouvement = this;
        ui.GetComponent<PlayerUI>().PlayerPI = this.GetComponent<PlayerInterrestPoint>();
    }
    #endregion
}

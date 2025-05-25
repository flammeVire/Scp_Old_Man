using FMODUnity;
using Fusion;
using Photon.Voice.Unity;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMouvement : NetworkBehaviour, ISpawned
{
    #region Data
    public Transform cam;
    public PlayerUI playerUI;
    public PlayerAnimation playerAnimation;
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
    public float decreaseStamina = 0.5f;
    public float increaseStamina = 0.1f;

    private CapsuleCollider playerCollider;
    [HideInInspector]public float gravity = 9.81f;
    private float rotationX = 0f;
    private Rigidbody body;

    [Header("Boolean")]
    public bool CanMove;
    public bool IsInPocketDim;
    public bool isGrounded;
    public bool isCrouching;
    public bool isJumping;
    public bool isRunning;
    public bool isMoving;
    public bool isTalking;


    [Header("Slow Corrosion")]
    private float corrosionSlow = 0.5f;
    private bool inCorrosionZone = false;

    [Header("SFX")]
    public PlayerSound PlayerSound;
    public StudioEventEmitter jump;
    public StudioEventEmitter talkieOn;
    public StudioEventEmitter dim;

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
            cam.AddComponent<StudioListener>();
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
            ManageStamina(isRunning);
        }
    }
    void Update()
    {
        if (HasInputAuthority)
        {
            if (!dim.IsPlaying() && IsInPocketDim)
            {
                dim.Play();
            }
            else if (dim.IsPlaying() && !IsInPocketDim)
            {
                dim.Stop();
            }
                Talk();
            Rotate();
            if (CanMove)
            {
                Movement();
                HandleJump();
                HandleCrouch();
                HandleRun();
                ManageSpeed();
            }
            /*
            if (!isMoving && !isCrouching && !isRunning)
            {
                playerAnimation.Idle();
            }
            */
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
            if (!isMoving)
            {
                PlayerSound.Rpc_Walk(true);
               // playerAnimation.Walk();
            }
            isMoving = true;
            //MattSounds jouer son marche
        }
        else if(horizontal == 0 && vertical == 0)
        {
            PlayerSound.Rpc_Walk(false);
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

                PlayerSound.Rpc_crouch();
                isCrouching = true;
                //MattSounds : jouer son accroupi
            }
            else
            {
                PlayerSound.Rpc_crouch();
                isCrouching = false;
                //MattSounds : jouer son relevé
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

                if (!isRunning)
                {
                    PlayerSound.Rpc_Run(true);
                }


                isRunning = true;


            }
            else if (Input.GetButtonUp("Run") || stamina <= 0)
            {
                Debug.Log("Not Running");
                PlayerSound.Rpc_Run(false);
                isRunning = false;
                if (isMoving)
                {
                    PlayerSound.Rpc_Walk(true);
                    //MattSounds jouer son marche
                }


            }
        }
        else
        {
            PlayerSound.Rpc_Run(false);
            isRunning = false;
            if (isMoving)
            {
                PlayerSound.Rpc_Walk(true);
                //MattSounds jouer son marche
            }
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
            if(isMoving == true)
            {
                PlayerSound.Rpc_Walk(true);
            }
            //MattSounds : jouer son course
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
            //MattSounds : joueur allumage talkie
            talkieOn.Play();
        }
        else if (Input.GetButtonUp("Talk"))
        {
            FindAnyObjectByType<Recorder>().RecordingEnabled = false;
            isTalking = false;
            Debug.Log("UnTalk");
            //MattSounds : jouer eteindre talkie
            talkieOn.Play();
        }
    }

    IEnumerator JumpDetection()
    {
        jump.Play();
        yield return new WaitForSeconds(0.2f);
        //MattSounds jouer son saut
        
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
        playerUI = ui.GetComponent<PlayerUI>();
        playerUI.mouvement = this;
        playerUI.PlayerPI = this.GetComponent<PlayerInterrestPoint>();
    }
    #endregion
}

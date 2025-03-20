using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class PlayerMovement : MonoBehaviour
{
    static readonly int isRifleEquipped = Animator.StringToHash("isRifleEquipped");
    static readonly int isPistolRunning = Animator.StringToHash("isPistolRunning");
    static readonly int isRifleRunning = Animator.StringToHash("isRifleRunning");
    static readonly int isPistolCrouching = Animator.StringToHash("isPistolCrouching");
    static readonly int isRifleCrouching = Animator.StringToHash("isRifleCrouching");
    static readonly int isPistolJumping = Animator.StringToHash("isPistolJumping");
    static readonly int isRifleJumping = Animator.StringToHash("isRifleJumping");

    [SerializeField] PlayerSO startingPlayer;
    [SerializeField] float crouchHeight = 0.86f;
    [SerializeField] Vector2 crouchCenter = new Vector2(0f, -0.16f);

    float originalBodyHeight;
    Vector2 originalBodyCenter;
    bool isAlive = true;
    bool isUsingRifle;
    bool allowRunning = true;
    
    //========= Movement Speed ========//
    float moveSpeed;
    float jumpSpeed;
    
    //========= Speed Boost ==========//
    private float originalMoveSpeed;
    private float speedBoostDuration;
    private float speedBoostTimer;
    private bool isSpeedBoosted = false;
    
    Vector2 moveInput;
    Rigidbody2D myRigidBody;
    Animator myAnimator;
    CapsuleCollider2D myBodyCollider;
    BoxCollider2D myFeetCollider;
    ActiveWeapon myActiveWeapon;
    Weapon myWeapon;
    AudioPlayer audioPlayer;
    PlayerSO currentPlayer;
    static PlayerMovement _instance;
    
    // Offsets for the rifle sprite for each animation state
    Dictionary<string, Vector2> rifleOffsets = new Dictionary<string, Vector2>()
    {
        {"Idling", new Vector2(0.23f, 0.35f)},
        {"Running", new Vector2(0.4f, 0.32f)},
        {"Crouching", new Vector2(0.211f, -0.1f)},
        {"Jumping", new Vector2(0.262f, 0.337f)}
    };

    // Offsets for the pistol sprite for each animation state
    Dictionary<string, Vector2> pistolOffsets = new Dictionary<string, Vector2>()
    {
        {"Idling", new Vector2(0.364f, 0.442f)},
        {"Running", new Vector2(0.48f, 0.40f)},
        {"Crouching", new Vector2(0.32f, 0.195f)},
        {"Jumping", new Vector2(0.30f, 0.44f)}
    };
    
    // Level spawn points
    Dictionary<string, Vector2> spawnPoints = new Dictionary<string, Vector2>()
    {
        {"City", new Vector2(-8f, -0.4f)},
        {"Moonlight", new Vector2(19f, -5f)},
        {"Hell", new Vector2(-4f, 2.7f)}
    };
    
    void Awake()
    {
        ManageSingleton();
    }
    
    // Applying singleton pattern
    void ManageSingleton()
    {
        if (_instance)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    
    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBodyCollider = GetComponent<CapsuleCollider2D>();
        myFeetCollider = GetComponent<BoxCollider2D>();
        myActiveWeapon = GetComponentInChildren<ActiveWeapon>();
        myWeapon = GetComponentInChildren<Weapon>();
        audioPlayer = FindFirstObjectByType<AudioPlayer>();
        
        // Set starting player stats
        if (currentPlayer == null)
        {
            currentPlayer = startingPlayer;
            SwitchPlayer(startingPlayer);
        }
        
        // Store the original height and center values
        originalBodyHeight = myBodyCollider.size.y;
        originalBodyCenter = myBodyCollider.offset;
        
        // Set pistol or animation mode based on the weapon
        SetAnimationMode();
    }
    
    void Update()
    {
        // if player is dead then deactivate controls
        if (!isAlive) return;
        Standing();
        if (allowRunning) Run();
        FlipSprite();
        AdjustWeaponPosition();
        
        // Handle speed boost timer
        if (isSpeedBoosted)
        {
            speedBoostTimer -= Time.deltaTime;
        
            // When timer expires, return to normal speed
            if (speedBoostTimer <= 0)
            {
                moveSpeed = originalMoveSpeed;
                isSpeedBoosted = false;
            }
        }
    }

    public void SwitchPlayer(PlayerSO player)
    {
        // Set the player movement and animation here
        myAnimator = GetComponent<Animator>();
        myAnimator.runtimeAnimatorController = player.animator;
        moveSpeed = player.moveSpeed;
        jumpSpeed = player.jumpSpeed;
        currentPlayer = player;
    }

    public void SetNewWeapon()
    {
        myWeapon = GetComponentInChildren<Weapon>();
        isUsingRifle = myActiveWeapon.IsRifle();
        
        if (isUsingRifle)
        {
            SetWeaponPosition(rifleOffsets["Idling"]);
        }
        else
        {
            SetWeaponPosition(pistolOffsets["Idling"]);
        }
    }
    
    public void SetAnimationMode()
    {
        isUsingRifle = myActiveWeapon.IsRifle();
        
        // Choose to play pistol or rifle animation
        if (!isUsingRifle) myAnimator.SetBool(isRifleEquipped, false);
        else myAnimator.SetBool(isRifleEquipped, true);
    }
    
    void OnShoot(InputValue value)
    {
        if (!isAlive) return;
        
        // Handle Shooting
        myActiveWeapon.HandleShootInput(value.isPressed);
    }

    void OnCrouch(InputValue value)
    {
        if (!isAlive) return;
        myAnimator.SetBool(isUsingRifle ? isRifleCrouching : isPistolCrouching, value.isPressed);
        
        // Adjust collider heights when crouching
        if (value.isPressed)
        {
            myBodyCollider.size = new Vector2(myBodyCollider.size.x, crouchHeight);
            myBodyCollider.offset = crouchCenter;
            allowRunning = false;
        }
        else
        {
            // Reset heights when standup
            myBodyCollider.size = new Vector2(myBodyCollider.size.x, originalBodyHeight);
            myBodyCollider.offset = originalBodyCenter;
            allowRunning = true;
        }
    }

    // Flip character based on the move direction
    void FlipSprite()
    {
        // 0 is still a positive value so use epsilon to get the smallest value possible
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidBody.linearVelocity.x) > Mathf.Epsilon;

        if (playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(myRigidBody.linearVelocity.x), 1f);
        }
    }

    // Get input value and store in moveInput
    void OnMove(InputValue value)
    {
        if (!isAlive) return;
        moveInput = value.Get<Vector2>();
    }
    
    // Get input value and store in jumpInput
    void OnJump(InputValue value)
    {   
        // if the player doesn't touch the ground then do nothing
        if (!isAlive) return;
        
        // Play animations
        myAnimator.SetBool(isUsingRifle ? isRifleJumping : isPistolJumping, true);
        
        // Check if the player is standing on the ground
        if (IsGroundTouching() && value.isPressed)
        {
            // Jump physics
            myRigidBody.linearVelocity += new Vector2(0f, jumpSpeed);
        }
    }

    // Check if the player is standing on the ground
    void Standing()
    {
        if (IsGroundTouching())
            myAnimator.SetBool(isUsingRifle ? isRifleJumping : isPistolJumping, false);
    }
    
    bool IsGroundTouching()
    {
        return myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"));
    }
    
    // Control character movement
    void Run()
    {
        // Move the player using values from player input
        Vector2 playerVelocity = new Vector2(moveInput.x * moveSpeed, myRigidBody.linearVelocity.y);
        myRigidBody.linearVelocity = playerVelocity;
        
        // Make sure the animations and is not played when player is standing still
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidBody.linearVelocity.x) > Mathf.Epsilon;

        if (playerHasHorizontalSpeed)
        {
            audioPlayer.StartRunningSound();
            myAnimator.SetBool(isUsingRifle ? isRifleRunning : isPistolRunning, true);
        }
        else
        {
            audioPlayer.StopRunningSound();
            myAnimator.SetBool(isUsingRifle ? isRifleRunning : isPistolRunning, false);
        }
        
    }

    public void SetAlive(bool aliveState)
    {
        isAlive = aliveState;
        if (aliveState)
        {
            // Reset animation state to default entry
            myAnimator.Rebind();
            myAnimator.Update(0f);
            SetAnimationMode();
        }
    }
    
    // Helper function to apply the offset to the weapon's position
    void SetWeaponPosition(Vector2 offset)
    {
        myWeapon.transform.localPosition = new Vector2(offset.x, offset.y);
    }
    
    void AdjustWeaponPosition()
    {
        myWeapon = GetComponentInChildren<Weapon>();
        
        // Determine the current state of the weapon (running, crouching, jumping)
        if (isUsingRifle)
        {
            // Adjust for Rifle states
            if (myAnimator.GetBool(isRifleRunning))
            {
                SetWeaponPosition(rifleOffsets["Running"]);
            }
            else if (myAnimator.GetBool(isRifleCrouching))
            {
                SetWeaponPosition(rifleOffsets["Crouching"]);
            }
            else if (myAnimator.GetBool(isRifleJumping))
            {
                SetWeaponPosition(rifleOffsets["Jumping"]);
            }
            else
            {
                SetWeaponPosition(rifleOffsets["Idling"]);
            }
        }
        else
        {
            // Adjust for Pistol states
            if (myAnimator.GetBool(isPistolRunning))
            {
                SetWeaponPosition(pistolOffsets["Running"]);
            }
            else if (myAnimator.GetBool(isPistolCrouching))
            {
                SetWeaponPosition(pistolOffsets["Crouching"]);
            }
            else if (myAnimator.GetBool(isPistolJumping))
            {
                SetWeaponPosition(pistolOffsets["Jumping"]);
            }
            else
            {
                SetWeaponPosition(pistolOffsets["Idling"]);
            }
        }
    }
    
    public void ApplySpeedBoost(float speedBoostMultiplier, float duration)
    {
        // Set the boost duration from the parameter
        speedBoostDuration = duration;
    
        // Only apply if not already boosted or apply new duration if it's longer
        if (!isSpeedBoosted || speedBoostDuration > speedBoostTimer)
        {
            if (!isSpeedBoosted)
            {
                // Store original speed for reference
                originalMoveSpeed = moveSpeed;
                // Apply the boost
                moveSpeed *= speedBoostMultiplier;
            }
    
            // Set the boost duration
            speedBoostTimer = speedBoostDuration;
            isSpeedBoosted = true;
        }
    }

    public void TeleportToSpawnPoint(string sceneName)
    {
        if (spawnPoints.ContainsKey(sceneName))
        {
            transform.position = spawnPoints[sceneName];
        }
        else
        {
            Debug.LogWarning("No spawn point defined for scene: " + sceneName);
        }
    }

    public void TeleportToPosition(Vector3 position)
    {
        transform.position = position;
    }

    public string GetCurrentPlayer()
    {
        return currentPlayer.Name;
    }
    
    void DestroyOnMenuScreen(Scene oldScene, Scene newScene)
    {
        var sceneName = SceneManager.GetActiveScene().name;
        
        if (sceneName == "MainMenu" || sceneName == "EndingMenu")
        {
            Destroy(this);
        }
    }
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        var sceneName = SceneManager.GetActiveScene().name;
        // Check if we should start or stop music based on new scene
        if (sceneName == "MainMenu" || sceneName == "Ending")
        {
            Destroy(this.gameObject);
        }
    }
    
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}

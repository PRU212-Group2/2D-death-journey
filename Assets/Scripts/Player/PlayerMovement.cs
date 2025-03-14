using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
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

    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float crouchHeight = 0.86f;
    [SerializeField] Vector2 crouchCenter = new Vector2(0f, -0.16f);

    float originalBodyHeight;
    Vector2 originalBodyCenter;
    Vector2 originalPistolPosition;
    Vector2 originalRiflePosition;
    bool isAlive = true;
    bool isUsingRifle;
    bool allowRunning = true;
    
    Vector2 moveInput;
    Rigidbody2D myRigidBody;
    Animator myAnimator;
    CapsuleCollider2D myBodyCollider;
    BoxCollider2D myFeetCollider;
    ActiveWeapon myActiveWeapon;
    PlayerHealth myPlayerHealth;
    Weapon myWeapon;
    
    // Offsets for the rifle sprite for each animation state
    Dictionary<string, Vector2> rifleOffsets = new Dictionary<string, Vector2>()
    {
        {"Running", new Vector2(0.355f, 0.38f)},
        {"Crouching", new Vector2(0.211f, 0.007f)},
        {"Jumping", new Vector2(0.262f, 0.337f)}
    };

    // Offsets for the pistol sprite for each animation state
    Dictionary<string, Vector2> pistolOffsets = new Dictionary<string, Vector2>()
    {
        {"Running", new Vector2(0.42f, 0.43f)},
        {"Crouching", new Vector2(0.32f, 0.195f)},
        {"Jumping", new Vector2(0.26f, 0.42f)}
    };
    
    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBodyCollider = GetComponent<CapsuleCollider2D>();
        myFeetCollider = GetComponent<BoxCollider2D>();
        myActiveWeapon = GetComponentInChildren<ActiveWeapon>();
        myPlayerHealth = GetComponent<PlayerHealth>();
        myWeapon = GetComponentInChildren<Weapon>();
        
        // Store the original height and center values
        originalBodyHeight = myBodyCollider.size.y;
        originalBodyCenter = myBodyCollider.offset;
        
        // Get weapon original position
        originalPistolPosition = myWeapon.transform.localPosition;
        originalRiflePosition = myWeapon.transform.localPosition;
        
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
    }

    void SetAnimationMode()
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
        
        // Make sure the animations is not played when player is standing still
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidBody.linearVelocity.x) > Mathf.Epsilon;
        myAnimator.SetBool(isUsingRifle ? isRifleRunning : isPistolRunning, playerHasHorizontalSpeed);
    }

    public void SetAlive(bool aliveState)
    {
        isAlive = aliveState;
    }
    
    // Helper function to apply the offset to the weapon's position
    void SetWeaponPosition(Vector2 offset)
    {
        myWeapon.transform.localPosition = new Vector2(offset.x, offset.y);
    }
    
    void AdjustWeaponPosition()
    {
        // Check which weapon is equipped using the myAnimator
        bool isRifle = myAnimator.GetBool(isRifleEquipped);

        // Determine the current state of the weapon (running, crouching, jumping)
        if (isRifle)
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
            else if (!myAnimator.GetBool(isRifleCrouching))
            {
                SetWeaponPosition(originalRiflePosition);
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
            else if (!myAnimator.GetBool(isPistolCrouching))
            {
                SetWeaponPosition(originalPistolPosition);
            }
        }
    }
}

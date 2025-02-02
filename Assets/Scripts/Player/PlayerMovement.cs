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
    static readonly int triggerDying = Animator.StringToHash("triggerDying");

    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpSpeed = 5f;

    bool isAlive = true;
    private bool isUsingRifle;
    
    Vector2 moveInput;
    Rigidbody2D myRigidBody;
    Animator myAnimator;
    CapsuleCollider2D myBodyCollider;
    BoxCollider2D myFeetCollider;
    ActiveWeapon myActiveWeapon;
    
    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBodyCollider = GetComponent<CapsuleCollider2D>();
        myFeetCollider = GetComponent<BoxCollider2D>();
        myActiveWeapon = GetComponentInChildren<ActiveWeapon>();
        SetAnimationMode();
    }
    
    void Update()
    {
        // if player is dead then deactivate controls
        if (!isAlive) return;
        Standing();
        Run();
        FlipSprite();
        Die();
    }

    void SetAnimationMode()
    {
        if (!isUsingRifle) myAnimator.SetBool(isRifleEquipped, false);
        else myAnimator.SetBool(isRifleEquipped, true);
    }
    
    public void SetRifleMode(bool isRifle)
    {
        isUsingRifle = isRifle;
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
    
    private void Die()
    {
        // if player collides with player then dies
        if (myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemy")))
        {
            isAlive = false;
            myAnimator.SetTrigger(triggerDying);
        };
    }
}

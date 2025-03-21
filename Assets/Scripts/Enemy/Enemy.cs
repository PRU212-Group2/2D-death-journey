using UnityEngine;

public class Enemy : MonoBehaviour
{
    private static readonly int isWalkingAnimation = Animator.StringToHash("IsWalking");
    private static readonly int isAttackingAnimation = Animator.StringToHash("IsAttacking");
    private static readonly int isHurtingAnimation = Animator.StringToHash("IsHurting");
    private static readonly int isDeadAnimation = Animator.StringToHash("IsDead");
    const string playerString = "Player";
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D myRigidbody;
    private EnemyHealth enemyHealth;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float moveRange = 0.8f;
    [SerializeField] private int damage = 20;
    Animator animator;
    AudioPlayer audioPlayer;
    
    private float leftBound;
    private float rightBound;
    private bool movingRight = true;
    private bool isDead = false;
    private bool isAttacking = false;
    private int lastHealth;

    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        enemyHealth = GetComponent<EnemyHealth>();
        audioPlayer = FindFirstObjectByType<AudioPlayer>();
        // Check if Animator is assigned before using it
        if (animator != null)
        {
            animator.SetBool(isWalkingAnimation, true);
        }
        else
        {
            Debug.LogWarning("Animator is not assigned on " + gameObject.name);
        }

        leftBound = transform.position.x - moveRange;
        rightBound = transform.position.x + moveRange;
    }

    void Update()
    {
        if (isDead || isAttacking) return;
        CheckHealth();
        MoveEnemy();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerString) && !isAttacking)
        {
            PlayerHealth playerHealth = other.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                FaceTarget(other.transform);
                playerHealth.TakeDamage(damage);
                animator.SetBool(isWalkingAnimation, false);
                animator.SetBool(isAttackingAnimation, true);

                audioPlayer.PlayEnemyAttackClip();
                isAttacking = true;
                myRigidbody.linearVelocity = Vector2.zero;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerString))
        {
            // Stop attacking and resume walking
            animator.SetBool(isAttackingAnimation, false);
            animator.SetBool(isWalkingAnimation, true);

            isAttacking = false;
            spriteRenderer.flipX = !movingRight;
        }
    }
    private void CheckHealth()
    {
        if (enemyHealth == null) return;

        int currentHealth = enemyHealth.GetCurrentHealth();
        
        if (currentHealth < lastHealth) // Damage taken
        {
            animator.SetTrigger(isHurtingAnimation);
            audioPlayer.PlayEnemyHurtClip();
            Invoke(nameof(ResumeWalking), 1f);
        }

        if (currentHealth <= 0 && !isDead) // Death logic
        {
            Die();
        }

        lastHealth = currentHealth; // Update last known health
    }

    private void ResumeWalking()
    {
        if (!isDead)
        {
            animator.ResetTrigger(isHurtingAnimation);
            animator.SetBool(isWalkingAnimation, true);
        }
    }
    private void FaceTarget(Transform target)
    {
        if (target.position.x > transform.position.x)
        {
            // Player is on the right → Face right
            spriteRenderer.flipX = false;
        }
        else
        {
            // Player is on the left → Face left
            spriteRenderer.flipX = true;
        }
    }
    void MoveEnemy()
    {
        if (isAttacking) return;
        
        if (movingRight)
        {
            myRigidbody.linearVelocity = new Vector2(moveSpeed, myRigidbody.linearVelocity.y);

            if (transform.position.x >= rightBound)
            {
                movingRight = false;
                Flip();
            }
        }
        else
        {
            myRigidbody.linearVelocity = new Vector2(-moveSpeed, myRigidbody.linearVelocity.y);

            if (transform.position.x <= leftBound)
            {
                movingRight = true;
                Flip();
            }
        }
    }

    void Flip()
    {
        // Flip the sprite
        spriteRenderer.flipX = !spriteRenderer.flipX;
    }

    public void Die()
    {
        isDead = true;
        myRigidbody.linearVelocity = Vector2.zero; // Stop movement immediately
        animator.SetTrigger(isDeadAnimation);
        
        Destroy(gameObject);
    }
}

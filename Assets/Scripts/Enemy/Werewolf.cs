using UnityEngine;

public class Werewolf : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D myRigidbody;
    private EnemyHealth enemyHealth;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float moveRange = 1f;
    [SerializeField] private int damage = 20;
    Animator animator;

    private float leftBound;
    private float rightBound;
    private bool movingRight = true;
    private bool isDead = false;
    private bool isAttacking = false;
    //  private bool lastMovingRight;
    private int lastHealth = 50;

    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        enemyHealth = GetComponent<EnemyHealth>();

        if (animator != null)
        {
            animator.SetBool("isWalking", true);
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
        //CheckHealth();
        MoveWerewolf();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isAttacking)
        {
            PlayerHealth playerHealth = other.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                isAttacking = true;

                animator.SetBool("isWalking", false);
                animator.SetBool("isAttacking", true);

                myRigidbody.linearVelocity = Vector2.zero;


                FaceTarget(other.transform);

                playerHealth.TakeDamage(damage);
            }
        }
        else if (other.CompareTag("Bullet") && !isAttacking)
        {
            enemyHealth.TakeDamage(1);

            if (enemyHealth.currentHealth > 0)
            {
                animator.SetBool("isHurting", true);
                Invoke(nameof(ResumeWalking), 0.5f);
            }
            else
            {
                Die();
            }
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isAttacking = false;
            animator.SetBool("isAttacking", false);
            animator.SetBool("isWalking", true);
        }
    }

    private void ResumeWalking()
    {
        if (!isDead)
        {
            animator.SetBool("isHurting", false);
            animator.SetBool("isWalking", true);
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

    void MoveWerewolf()
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
        myRigidbody.linearVelocity = Vector2.zero;
        Destroy(gameObject);
    }
}

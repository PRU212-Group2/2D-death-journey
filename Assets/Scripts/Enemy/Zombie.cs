using UnityEngine;

public class Zombie : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D myRigidbody;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float moveRange = 0.8f;
    public Animator animator;

    private float leftBound;
    private float rightBound;
    private bool movingRight = true;
    private bool isDead = false;

    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Check if Animator is assigned before using it
        if (animator != null)
        {
            animator.SetBool("IsWalk", true);
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
        if (isDead) return;

        MoveZombie();
    }

    void MoveZombie()
    {
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

        if (animator != null)
        {
            animator.SetBool("IsDead", true);
        }
    }
}

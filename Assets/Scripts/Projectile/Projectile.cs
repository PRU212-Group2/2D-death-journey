using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float bulletSpeed = 1f;
    
    Rigidbody2D myRigidBody;
    PlayerMovement player;
    float xSpeed;
    
    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        
        // Find the player
        player = FindFirstObjectByType<PlayerMovement>();
        
        // Define the projectile speed that travels from the player
        xSpeed = player.transform.localScale.x * bulletSpeed;
    }
    
    void Update()
    {
        Fly();
    }

    private void Fly()
    {
        // Projectile travels forward
        myRigidBody.linearVelocity = new Vector2(xSpeed, 0f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        HitEnemy(other);
    }

    void HitEnemy(Collider2D other)
    {
        // Destroy enemy and destroy itself
        if (other.gameObject.CompareTag("Enemy"))
        {
            Destroy(other.gameObject);
        }

        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        HitWall();
    }

    void HitWall()
    {
        // Destroy the projectile
        Destroy(gameObject);
    }
}

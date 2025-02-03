using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float bulletSpeed = 1f;
    [SerializeField] ParticleSystem hitVFX;
    
    Rigidbody2D myRigidBody;
    PlayerMovement player;
    WeaponSO weaponSO;
    float xSpeed;
    int damage;
    
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

    void Fly()
    {
        // Projectile travels forward
        myRigidBody.linearVelocity = new Vector2(xSpeed, 0f);
    }
    public void SetDamage(int projectileDamage)
    {
        this.damage = projectileDamage;
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        PlayHitVFX();
        HitEnemy(other);
    }

    void HitEnemy(Collider2D other)
    {
        // Destroy enemy and destroy itself
        if (other.gameObject.CompareTag("Enemy"))
        {
            EnemyHealth enemyHealth = other.gameObject.GetComponent<EnemyHealth>();
            enemyHealth.TakeDamage(damage);
        }
        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        PlayHitVFX();
        HitWall();
    }

    void HitWall()
    {
        // Destroy the projectile
        Destroy(gameObject);
    }

    void PlayHitVFX()
    {
        if (hitVFX != null)
        {
            ParticleSystem vfxInstance = Instantiate(hitVFX, transform.position, transform.rotation);
            Destroy(vfxInstance.gameObject, vfxInstance.main.duration); // Destroy after the effect finishes
        }
    }
}

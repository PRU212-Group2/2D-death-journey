using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float bulletSpeed = 1f;
    [SerializeField] ParticleSystem hitVFX;
    [SerializeField] ParticleSystem bloodVFX;
    
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
        damage = projectileDamage;
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            HitEnemy(other);
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Ground")) // Check for Ground layer
        {
            HitWall();
        }
    }

    void HitEnemy(Collider2D other)
    {
        // Destroy enemy and destroy itself
        if (other.gameObject.CompareTag("Enemy"))
        {
            PlayBloodVFX();
            EnemyHealth enemyHealth = other.gameObject.GetComponent<EnemyHealth>();
            enemyHealth.TakeDamage(damage);
        }
        Destroy(gameObject);
    }

    void HitWall()
    {
        // Destroy the projectile
        PlayHitVFX();
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
    
    void PlayBloodVFX()
    {
        if (bloodVFX != null)
        {
            ParticleSystem vfxInstance = Instantiate(bloodVFX, transform.position, transform.rotation);
            Destroy(vfxInstance.gameObject, vfxInstance.main.duration); // Destroy after the effect finishes
        }
    }
}

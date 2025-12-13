using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float bulletSpeed = 30f;
    [SerializeField] ParticleSystem hitVFX;
    [SerializeField] ParticleSystem bloodVFX;

    private Rigidbody2D myRigidBody;
    private int damage;
    private ObjectPool<Projectile> _myPool;

    private void Awake()
    {
        // Awake runs once. Good for caching references.
        myRigidBody = GetComponent<Rigidbody2D>();
    }

    public void Init(ObjectPool<Projectile> pool, float directionX, int weaponDamage)
    {
        _myPool = pool;
        damage = weaponDamage;

        // Reset Physics
        myRigidBody.linearVelocity = Vector2.zero;
        myRigidBody.angularVelocity = 0f;

        // Apply Velocity immediately
        myRigidBody.linearVelocity = new Vector2(directionX * bulletSpeed, 0f);

        // Safety: Auto-destroy after 3 seconds if it hits nothing
        CancelInvoke(nameof(ReturnSelf));
        Invoke(nameof(ReturnSelf), 3f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            HitEnemy(other);
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            HitWall();
        }
    }

    void HitEnemy(Collider2D other)
    {
        PlayBloodVFX();
        EnemyHealth enemyHealth = other.gameObject.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damage);
        }
        ReturnSelf();
    }

    void HitWall()
    {
        PlayHitVFX();
        ReturnSelf();
    }

    private void ReturnSelf()
    {
        // Cancel the auto-destroy timer so it doesn't fire while inside the pool
        CancelInvoke(nameof(ReturnSelf));

        if (_myPool != null)
        {
            _myPool.ReturnToPool(this);
        }
        else
        {
            gameObject.SetActive(false); // Fallback
        }
    }

    void PlayHitVFX()
    {
        if (hitVFX != null)
        {
            ParticleSystem vfxInstance = Instantiate(hitVFX, transform.position, transform.rotation);
            Destroy(vfxInstance.gameObject, vfxInstance.main.duration);
        }
    }

    void PlayBloodVFX()
    {
        if (bloodVFX != null)
        {
            ParticleSystem vfxInstance = Instantiate(bloodVFX, transform.position, transform.rotation);
            Destroy(vfxInstance.gameObject, vfxInstance.main.duration);
        }
    }
}
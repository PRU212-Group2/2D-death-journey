using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] int startingHealth = 3;
    
    public int currentHealth;
    
    void Awake()
    {
        currentHealth = startingHealth;    
    }
    
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            // Kill enemy if health reaches 0
            Death();
        }
    }

    public void Death()
    {
        // Destroy enemy object
        Destroy(this.gameObject);
    }
}

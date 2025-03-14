using UnityEngine;

public abstract class Hazard : MonoBehaviour
{
    [SerializeField] int damage = 50;
    
    const string playerString = "Player";
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        
        if (other.gameObject.CompareTag(playerString))
        {
            playerHealth.TakeDamage(damage);
        }
    }
    
    protected abstract void OnContact();
}

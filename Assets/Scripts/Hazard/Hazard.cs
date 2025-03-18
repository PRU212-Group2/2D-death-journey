using UnityEngine;

public abstract class Hazard : MonoBehaviour
{
    [SerializeField] int damage = 50;
    
    const string playerString = "Player";
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        BoxCollider2D feetCollider = other.GetComponent<BoxCollider2D>();
        
        if (other.gameObject.CompareTag(playerString) 
            && feetCollider.IsTouchingLayers(LayerMask.GetMask("Hazards")))
        {
            playerHealth.TakeDamage(damage);
        }
    }
    
    protected abstract void OnContact();
}

using UnityEngine;

public class CityTrap : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

        if (other.tag == "Player")
        {
            // Trigger game over function
            playerHealth.TakeDamage(50);
        }
    }
}

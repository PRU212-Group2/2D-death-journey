using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHealth : MonoBehaviour
{
    static readonly int triggerDying = Animator.StringToHash("triggerDying");

    [Range(1, 100)]
    [SerializeField] int startingHealth = 100;

    int currentHealth;
    Animator animator;
    PlayerMovement playerMovement;
    AudioPlayer audioPlayer;
    ActiveWeapon activeWeapon;

    void Awake()
    {
        currentHealth = startingHealth;
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        audioPlayer = FindFirstObjectByType<AudioPlayer>();
        activeWeapon = GetComponentInChildren<ActiveWeapon>();
    }

    public void TakeDamage(int amount)
    {
        // Decrease health based on damage taken
        currentHealth -= amount;

        // If the player dies then play death animation
        if (currentHealth <= 0)
        {
            PlayerGameOver();
        }
    }

    void PlayerGameOver()
    {
        // Play death animation, sfx and deactivate input
        audioPlayer.PlayDeathClip();
        activeWeapon.gameObject.SetActive(false);
        playerMovement.SetAlive(false);
        animator.SetTrigger(triggerDying);
    }
}

using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;  // Add this for UI elements

public class PlayerHealth : MonoBehaviour
{
    static readonly int triggerDying = Animator.StringToHash("triggerDying");
    
    [Range(1, 100)]
    [SerializeField] int startingHealth = 100;
    [SerializeField] int lowHealthThreshold = 30;  // New threshold variable
    [SerializeField] Image lowHealthEffectImage;   // Reference to your UI image
    
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
    
    void Update()
    {
        UpdateLowHealthEffect();
    }
    
    void UpdateLowHealthEffect()
    {
        if (lowHealthEffectImage == null) return;
        
        if (currentHealth <= lowHealthThreshold && currentHealth > 0)
        {
            lowHealthEffectImage.GameObject().SetActive(true);
        }
        else
        {
            // Hide the effect when health is above threshold or player is dead
            lowHealthEffectImage.GameObject().SetActive(false);
        }
    }
    
    public int GetHealth()
    {
        return currentHealth;
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
        else
        {
            audioPlayer.PlayHurtClip();
        }
    }
    
    void PlayerGameOver()
    {
        // Play death animation, sfx and deactivate input
        audioPlayer.PlayDeathClip();
        audioPlayer.StopRunningSound();
        activeWeapon.gameObject.SetActive(false);
        playerMovement.SetAlive(false);
        animator.SetTrigger(triggerDying);
    }
}
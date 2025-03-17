using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;  // Add this for UI elements

public class PlayerHealth : MonoBehaviour
{
    static readonly int triggerDying = Animator.StringToHash("triggerDying");
    
    [SerializeField] PlayerSO startingPlayer;
    [SerializeField] Image lowHealthEffectImage;
    
    int startingHealth;
    int lowHealthThreshold;
    int currentHealth;
    Animator animator;
    PlayerMovement playerMovement;
    AudioPlayer audioPlayer;
    ActiveWeapon activeWeapon;
    
    void Awake()
    {
        SwitchPlayer(startingPlayer);
        
        currentHealth = startingHealth;
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        audioPlayer = FindFirstObjectByType<AudioPlayer>();
        activeWeapon = GetComponentInChildren<ActiveWeapon>();
    }
    
    public void SwitchPlayer(PlayerSO player)
    {
        startingHealth = player.playerHealth;
        lowHealthThreshold = player.lowHealthThreshold;
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

    public int GetMaxHealth()
    {
        return startingHealth;
    }
    
    public void Heal(int amount)
    {
        currentHealth += amount;
        
        // If the player dies then play death animation
        if (currentHealth > startingHealth)
        {
            currentHealth = startingHealth;
        }
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
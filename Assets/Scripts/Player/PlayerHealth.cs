using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

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
    PlayerHealth _instance;
    
    // Immortality variables
    private bool isImmortal = false;
    private float immortalityTimer = 0f;
    private float immortalityDuration = 0f;
    
    void Awake()
    {
        ManageSingleton();
        SwitchPlayer(startingPlayer);
        
        currentHealth = startingHealth;
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        audioPlayer = FindFirstObjectByType<AudioPlayer>();
        activeWeapon = GetComponentInChildren<ActiveWeapon>();
    }
    
    // Applying singleton pattern
    void ManageSingleton()
    {
        if (_instance)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    
    public void SwitchPlayer(PlayerSO player)
    {
        startingHealth = player.playerHealth;
        lowHealthThreshold = player.lowHealthThreshold;
    }
    
    void Update()
    {
        UpdateLowHealthEffect();
        UpdateImmortality();
    }
    
    void UpdateLowHealthEffect()
    {
        if (lowHealthEffectImage == null) return;
        
        if (currentHealth <= lowHealthThreshold && currentHealth > 0)
        {
            lowHealthEffectImage.gameObject.SetActive(true);
        }
        else
        {
            // Hide the effect when health is above threshold or player is dead
            lowHealthEffectImage.gameObject.SetActive(false);
        }
    }
    
    void UpdateImmortality()
    {
        if (isImmortal)
        {
            immortalityTimer -= Time.deltaTime;
            
            // When timer expires, return to normal
            if (immortalityTimer <= 0)
            {
                isImmortal = false;
            }
        }
    }
    
    public void ApplyImmortal(float duration)
    {
        // Set the immortality duration
        immortalityDuration = duration;
        
        // Only apply if not already immortal or apply new duration if it's longer
        if (!isImmortal || immortalityDuration > immortalityTimer)
        {
            immortalityTimer = immortalityDuration;
            isImmortal = true;
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
        // If player is immortal, ignore damage
        if (isImmortal) return;
        
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

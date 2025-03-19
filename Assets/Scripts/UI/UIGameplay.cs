using TMPro;
using UnityEngine;
using Slider = UnityEngine.UI.Slider;

public class UIGameplay : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] Slider healthSlider;

    [Header("Ammo")]
    [SerializeField] TextMeshProUGUI ammoText;
    
    [Header("Cash")]
    [SerializeField] TextMeshProUGUI cashText;
    
    [Header("Gameplay")]
    [SerializeField] GameObject pauseMenu;
    
    ActiveWeapon activeWeapon;
    CurrencyManager currencyManager;
    PlayerHealth playerHealth;
    UIGameplay _instance;

    public bool isPaused = false;
    float previousTimeScale = 1;

    void Awake()
    {
        ManageSingleton();
        currencyManager = FindFirstObjectByType<CurrencyManager>();
        activeWeapon = FindFirstObjectByType<ActiveWeapon>();
        playerHealth = FindFirstObjectByType<PlayerHealth>();
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
    
    private void Start()
    {
        healthSlider.maxValue = playerHealth.GetHealth();
    }

    void Update()
    {
        // Update health bar and score text on the UI
        healthSlider.value = playerHealth.GetHealth();
        ammoText.text = activeWeapon.GetAmmo().ToString("000");
        cashText.text = currencyManager.GetCash().ToString("000000");
        
        // Check if the player press escape key then pause the game
        if (Input.GetKeyDown(KeyCode.Escape)) TogglePause();
    }
    
    void TogglePause()
    {
        if (Time.timeScale > 0)
        {
            isPaused = true;
            previousTimeScale = Time.timeScale;
            Time.timeScale = 0;
            pauseMenu.SetActive(true);
        }
        else if (Time.timeScale == 0)
        {
            isPaused = false;
            Time.timeScale = previousTimeScale;
            pauseMenu.SetActive(false);
        }
    }

    public void UnPause()
    {
        isPaused = false;
        Time.timeScale = previousTimeScale;
    }
}

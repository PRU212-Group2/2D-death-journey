using System;
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
    
    ActiveWeapon activeWeapon;
    CurrencyManager currencyManager;
    PlayerHealth playerHealth;

    void Awake()
    {
        currencyManager = FindFirstObjectByType<CurrencyManager>();
        activeWeapon = FindFirstObjectByType<ActiveWeapon>();
        playerHealth = FindFirstObjectByType<PlayerHealth>();
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
    }
}

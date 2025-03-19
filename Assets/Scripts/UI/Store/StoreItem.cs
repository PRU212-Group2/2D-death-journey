using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class StoreItem : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] int price;
    [SerializeField] TextMeshProUGUI priceText;
    
    CurrencyManager currencyManager;
    AudioPlayer audioPlayer;

    void Awake()
    {
        audioPlayer = FindFirstObjectByType<AudioPlayer>();
    }
    
    void Update()
    {
        priceText.text = price.ToString();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            currencyManager = FindFirstObjectByType<CurrencyManager>();
            int currentCash = currencyManager.GetCash();

            // Check if the player has enough cash
            if (currentCash < price)
            {
                return;
            }
            
            // Deduct player's cash
            currencyManager.AddCash(-price);
            
            // Play buy audio
            audioPlayer.PlayPickupClip();
            
            OnClick();
        }
    }

    protected abstract void OnClick();
}

using UnityEngine;

public class Money : Pickup
{
    [SerializeField] int cash = 200;
    
    AudioPlayer audioPlayer;
    CurrencyManager currencyManager;

    void Start()
    {
        audioPlayer = FindFirstObjectByType<AudioPlayer>();
        currencyManager = FindFirstObjectByType<CurrencyManager>();
    }

    protected override void OnPickup()
    {
        currencyManager.AddCash(cash);
        audioPlayer.PlayPickupClip();
    }
}

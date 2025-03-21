using UnityEngine;

public class Money : MonoBehaviour
{
    const string playerString = "Player";
    
    [SerializeField] int cash = 200;
    
    AudioPlayer audioPlayer;
    CurrencyManager currencyManager;

    void Start()
    {
        audioPlayer = FindFirstObjectByType<AudioPlayer>();
        currencyManager = FindFirstObjectByType<CurrencyManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(playerString))
        {
            Destroy(gameObject);
            OnPickup();
        }
    }
    
    void OnPickup()
    {
        currencyManager.AddCash(cash);
        audioPlayer.PlayPickupClip();
    }
}

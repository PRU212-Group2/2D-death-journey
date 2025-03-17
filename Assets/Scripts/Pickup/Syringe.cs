using UnityEngine;

public class Syringe : Pickup
{
    AudioPlayer audioPlayer;

    void Start()
    {
        audioPlayer = FindFirstObjectByType<AudioPlayer>();
    }
    
    protected override void OnPickup()
    {
        audioPlayer.PlayInventoryClip();
    }
}

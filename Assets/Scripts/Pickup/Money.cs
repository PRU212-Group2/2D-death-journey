public class Money : Pickup
{
    AudioPlayer audioPlayer;

    void Start()
    {
        audioPlayer = FindFirstObjectByType<AudioPlayer>();  
    }

    protected override void OnPickup()
    {
        audioPlayer.PlayPickupClip();
    }
}

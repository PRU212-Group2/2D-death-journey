using UnityEngine;
using UnityEngine.Serialization;

public class AudioPlayer : MonoBehaviour
{
    [Header("Pistol")] 
    [SerializeField] AudioClip pistolClip;
    [SerializeField] [Range(0f, 1f)] float pistolVolume = 1f;
    
    [Header("Rifle")] 
    [SerializeField] AudioClip rifleClip;
    [SerializeField] [Range(0f, 1f)] float rifleVolume = 1f;
    
    [Header("Death")]
    [SerializeField] AudioClip deathClip;

    [SerializeField] [Range(0f, 1f)] private float deathVolume = 1f;
    
    [Header("Pickup")]
    [SerializeField] AudioClip pickupClip;
    [SerializeField] [Range(0f, 1f)] float pickupVolume = 1f;
    
    [Header("Songs")]
    [SerializeField] AudioClip[] songs; 
    [SerializeField] [Range(0f, 1f)] float songVolume = 1f;

    int currentSongIndex = 0;
    AudioSource audioSource;
    private AudioSource rifleSource;
    private bool isRifleShootingPlaying = false;
    
    static AudioPlayer _instance;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
        CreateRifleAudioSource();

        // Play the first song on start
        if (songs.Length > 0)
        {
            PlayCurrentSong();
        }
    }

    private void CreateRifleAudioSource()
    {
        // Create a dedicated audio source for pistol sound
        rifleSource = gameObject.AddComponent<AudioSource>();
        rifleSource.clip = rifleClip;
        rifleSource.volume = rifleVolume;
        rifleSource.loop = true;
    }

    void Awake()
    {
        ManageSingleton();
    }
    
    void Update()
    {
        // Continuously check if the song is playing and switch to the next one
        CheckAndPlayNextSong();
    }

    void PlayCurrentSong()
    {
        // Set the current song and play it
        audioSource.clip = songs[currentSongIndex];
        audioSource.volume = songVolume;
        audioSource.Play();
    }

    void CheckAndPlayNextSong()
    {
        // Check if the current song is finished playing
        if (!audioSource.isPlaying)
        {
            // Move to the next song and play it
            currentSongIndex = (currentSongIndex + 1) % songs.Length;
            PlayCurrentSong();
        }
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

    public void PlayPistolClip()
    {
        PlayClip(pistolClip, pistolVolume);
    }
    
    public void StartRifleShootingSound()
    {
        if (!isRifleShootingPlaying && rifleSource != null)
        {
            rifleSource.Play();
            isRifleShootingPlaying = true;
        }
    }
    
    public void StopRifleShootingSound()
    {
        if (isRifleShootingPlaying && rifleSource != null)
        {
            rifleSource.Stop();
            isRifleShootingPlaying = false;
        }
    }

    public void PlayDeathClip()
    {
        PlayClip(deathClip, deathVolume);
    }
    
    public void PlayPickupClip()
    {
        PlayClip(pickupClip, pickupVolume);
    }

    void PlayClip(AudioClip clip, float volume)
    {
        if (clip)
        {
            // Play audio clip at camera with defined volume
            Vector3 cameraPos = Camera.main.transform.position;
            AudioSource.PlayClipAtPoint(clip, cameraPos, volume);
        }
    }
}

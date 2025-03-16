using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.SceneManagement; // Add this to access scene information

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
    
    [Header("UI")]
    [SerializeField] AudioClip buttonHoverClip;
    [SerializeField] [Range(0f, 1f)] float buttonHoverVolume = 1f;
    [SerializeField] AudioClip buttonClickClip;
    [SerializeField] [Range(0f, 1f)] float buttonClickVolume = 1f;
    [SerializeField] AudioClip buttonStartGameClip;
    [SerializeField] [Range(0f, 1f)] float buttonStartGameVolume = 1f;
    [SerializeField] AudioClip buttonBackClip;
    [SerializeField] [Range(0f, 1f)] float buttonBackVolume = 1f;
    
    [Header("Songs")]
    [SerializeField] AudioClip[] songs; 
    [SerializeField] [Range(0f, 1f)] float songVolume = 1f;
    [SerializeField] string mainMenuSceneName = "MainMenu";
    [SerializeField] string instructionsSceneName = "Instructions";
    [SerializeField] string gameOverSceneName = "GameOver";

    int currentSongIndex = 0;
    AudioSource audioSource;
    private AudioSource rifleSource;
    private bool isRifleShootingPlaying = false;
    
    private float soundEffectVolume = 1f;
    private float musicVolume = 1f;
    
    static AudioPlayer _instance;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
        CreateRifleAudioSource();

        // Only play songs if we're not in the MainMenu scene
        if (songs.Length > 0 && !IsMenusScene())
        {
            PlayCurrentSong();
        }
    }

    public float GetSoundEffectVolume()
    {
        return soundEffectVolume;
    }

    public void SetSoundEffectVolume(float value)
    {
        soundEffectVolume = value;
        AdjustSoundEffectVolumes();
    }

    public float GetMusicVolume()
    {
        return musicVolume;
    }

    public void SetMusicVolume(float value)
    {
        musicVolume = value;
        AdjustSongVolumes();
    }
    
    private void AdjustSoundEffectVolumes()
    {
        // Apply the new sound effect volume to all sound effects
        pistolVolume = soundEffectVolume;
        rifleVolume = soundEffectVolume;
        deathVolume = soundEffectVolume;
        buttonHoverVolume = soundEffectVolume;
        buttonClickVolume = soundEffectVolume;
        buttonStartGameVolume = soundEffectVolume;
        buttonBackVolume = soundEffectVolume;
    }

    private void AdjustSongVolumes()
    {
        // Apply the new music volume
        songVolume = musicVolume;
    }
    
    // New method to check if current scene is MainMenu
    private bool IsMenusScene()
    {
        return SceneManager.GetActiveScene().name == mainMenuSceneName
            || SceneManager.GetActiveScene().name == instructionsSceneName
            || SceneManager.GetActiveScene().name == gameOverSceneName;
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
        // Only check for next song if we're not in MainMenu
        if (!IsMenusScene())
        {
            CheckAndPlayNextSong();
        }
        else if (audioSource.isPlaying)
        {
            // Stop any playing songs if we are in MainMenu
            audioSource.Stop();
        }
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

    // Since we're using DontDestroyOnLoad, add a scene change listener
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Check if we should start or stop music based on new scene
        if (!IsMenusScene() && songs.Length > 0 && !audioSource.isPlaying)
        {
            PlayCurrentSong();
        }
        else if (IsMenusScene() && audioSource.isPlaying)
        {
            audioSource.Stop();
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

    public void PlayButtonHoverClip()
    {
        PlayClip(buttonHoverClip, buttonHoverVolume);
    }
    
    public void PlayButtonClickClip()
    {
        PlayClip(buttonClickClip, buttonClickVolume);
    }
    
    public void PlayButtonStartGameClip()
    {
        PlayClip(buttonStartGameClip, buttonStartGameVolume);
    }
    
    public void PlayButtonBackClip()
    {
        PlayClip(buttonBackClip, buttonBackVolume);
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
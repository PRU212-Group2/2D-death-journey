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
    
    [Header("Running")]
    [SerializeField] AudioClip runningClip;
    [SerializeField] [Range(0f, 1f)] float runningVolume = 1f;
    
    [Header("Hurt")]
    [SerializeField] AudioClip hurtClip;
    [SerializeField] [Range(0f, 1f)] float hurtVolume = 1f;
    
    [Header("Enemy")]
    [SerializeField] AudioClip enemyAttackClip;
    [SerializeField] [Range(0f, 1f)] float enemyAttackVolume = 1f;
    [SerializeField] AudioClip enemyHurtClip;
    [SerializeField] [Range(0f, 1f)] float enemyHurtVolume = 1f;
    
    [Header("Unlock")]
    [SerializeField] AudioClip unlockClip;
    [SerializeField] [Range(0f, 1f)] float unlockVolume = 1f;
    
    [Header("UI")]
    [SerializeField] AudioClip buttonHoverClip;
    [SerializeField] [Range(0f, 1f)] float buttonHoverVolume = 1f;
    [SerializeField] AudioClip buttonClickClip;
    [SerializeField] [Range(0f, 1f)] float buttonClickVolume = 1f;
    [SerializeField] AudioClip buttonStartGameClip;
    [SerializeField] [Range(0f, 1f)] float buttonStartGameVolume = 1f;
    [SerializeField] AudioClip buttonBackClip;
    [SerializeField] [Range(0f, 1f)] float buttonBackVolume = 1f;
    [SerializeField] AudioClip inventoryClip;
    [SerializeField] [Range(0f, 1f)] float inventoryVolume = 1f;
    [SerializeField] AudioClip useItemClip;
    [SerializeField] [Range(0f, 1f)] float useItemVolume = 1f;
    [SerializeField] AudioClip saveClip;
    [SerializeField] [Range(0f, 1f)] float saveVolume = 1f;
    
    [Header("Songs")]
    [SerializeField] AudioClip[] songs; 
    [SerializeField] [Range(0f, 1f)] float songVolume = 1f;
    [SerializeField] string mainMenuSceneName = "MainMenu";

    int currentSongIndex = 0;
    AudioSource audioSource;
    private AudioSource rifleSource;
    private AudioSource runningSource;
    private bool isRifleShootingPlaying = false;
    private bool isRunningPlaying = false;
    
    private float soundEffectVolume = 1f;
    private float musicVolume = 1f;
    
    static AudioPlayer _instance;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
        CreateRunningAudioSource();
        CreateRifleAudioSource();

        // Only play songs if we're not in the MainMenu scene
        if (songs.Length > 0 && !IsMenusScene())
        {
            PlayCurrentSong();
        }
    }

    /// <summary>
    /// SOUND SETTINGS
    /// </summary>
    /// <returns></returns>
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
        runningVolume = soundEffectVolume;
        hurtVolume = soundEffectVolume;
        inventoryVolume = soundEffectVolume;
        enemyHurtVolume = soundEffectVolume;
        enemyAttackVolume = soundEffectVolume;
    }

    private void AdjustSongVolumes()
    {
        // Apply the new music volume
        songVolume = musicVolume;
    }
    
    // New method to check if current scene is MainMenu
    private bool IsMenusScene()
    {
        return SceneManager.GetActiveScene().name == mainMenuSceneName;
    }

    /// <summary>
    /// CREATE AUDIO SOURCE FOR RUNNING AND SHOOTING AS THEY ARE CONTINUOUS
    /// </summary>
    private void CreateRifleAudioSource()
    {
        // Create a dedicated audio source for pistol sound
        rifleSource = gameObject.AddComponent<AudioSource>();
        rifleSource.clip = rifleClip;
        rifleSource.volume = rifleVolume;
        rifleSource.loop = true;
    }

    private void CreateRunningAudioSource()
    {
        // Create a dedicated audio source for pistol sound
        runningSource = gameObject.AddComponent<AudioSource>();
        runningSource.clip = runningClip;
        runningSource.volume = runningVolume;
        runningSource.loop = true;
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

    /// <summary>
    /// PLAY SONGS AND ROTATE
    /// </summary>
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

    
    // Since we're using DontDestroyOnLoad, add a scene change listener
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// PLAY SONGS ONLY IN GAMEPLAY LEVELS
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="mode"></param>
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

    /// <summary>
    /// PLAY AUDIO CLIPS SECTION
    /// </summary>
    public void PlayPistolClip()
    {
        PlayClip(pistolClip, pistolVolume);
    }
    
    //====== SEPARATE AUDIO SOURCE FOR CONTINUOUS AUDIO =======//
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
    
    public void StartRunningSound()
    {
        if (!isRunningPlaying && runningSource != null)
        {
            runningSource.Play();
            isRunningPlaying = true;
        }
    }
    
    public void StopRunningSound()
    {
        if (isRunningPlaying && runningSource != null)
        {
            runningSource.Stop();
            isRunningPlaying = false;
        }
    }

    //====== PLAYER AND ENEMY SOUND EFFECTS =======//
    public void PlayDeathClip()
    {
        PlayClip(deathClip, deathVolume);
    }
    
    public void PlayPickupClip()
    {
        PlayClip(pickupClip, pickupVolume);
    }

    public void PlayHurtClip()
    {
        PlayClip(hurtClip, hurtVolume);
    }

    public void PlayEnemyAttackClip()
    {
        PlayClip(enemyAttackClip, enemyAttackVolume);
    }
    
    public void PlayEnemyHurtClip()
    {
        PlayClip(enemyHurtClip, enemyHurtVolume);
    }
    
    public void PlayDoorUnlockClip()
    {
        PlayClip(unlockClip, unlockVolume);
    }
    
    //====== UI SOUND EFFECTS =======//
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
    
    public void PlayInventoryClip()
    {
        PlayClip(inventoryClip, inventoryVolume);
    }
    
    public void PlayUseItemClip()
    {
        PlayClip(useItemClip, useItemVolume);
    }
    
    public void PlaySaveClip()
    {
        PlayClip(saveClip, saveVolume);
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
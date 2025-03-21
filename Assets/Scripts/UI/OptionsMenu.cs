using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] Slider soundEffectSlider;  // Assign in the Inspector
    [SerializeField] Slider musicSlider;        // Assign in the Inspector
    private AudioPlayer audioPlayer;

    void Start()
    {
        // Find the AudioManager in the scene (ensure it's in the same scene)
        audioPlayer = FindAnyObjectByType<AudioPlayer>();

        if (audioPlayer != null)
        {
            // Initialize the sliders with the current values from the AudioManager
            soundEffectSlider.value = audioPlayer.GetSoundEffectVolume();
            musicSlider.value = audioPlayer.GetMusicVolume();

            // Add listeners to sliders to handle changes
            soundEffectSlider.onValueChanged.AddListener(OnSoundEffectSliderChanged);
            musicSlider.onValueChanged.AddListener(OnMusicSliderChanged);
        }
        else
        {
            Debug.LogWarning("AudioManager not found in the scene!");
        }
    }

    private void OnSoundEffectSliderChanged(float value)
    {
        if (audioPlayer != null)
        {
            audioPlayer.SetSoundEffectVolume(value);  // Update AudioManager sound effect volume
        }
    }

    private void OnMusicSliderChanged(float value)
    {
        if (audioPlayer != null)
        {
            audioPlayer.SetMusicVolume(value);  // Update AudioManager music volume
        }
    }
}
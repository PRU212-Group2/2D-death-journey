using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Button : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    AudioPlayer audioPlayer;
    
    [Header("Button State")]
    [SerializeField] private Sprite hoverSprite;
    
    private Image buttonImage;
    private bool isSelected = false;

    void Start()
    {
        audioPlayer = FindFirstObjectByType<AudioPlayer>();
        buttonImage = GetComponent<Image>();
        
        // Make sure the button starts transparent if not selected
        if (buttonImage != null && !isSelected)
        {
            buttonImage.color = new Color(1f, 1f, 1f, 0f);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isSelected)
        {
            PlayHoverSound();
            ShowSprite();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Hide sprite only if not selected
        if (!isSelected)
        {
            HideSprite();
        }
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        
        if (selected)
        {
            ShowSprite();
        }
        else
        {
            HideSprite();
        }
    }
    
    private void ShowSprite()
    {
        if (buttonImage != null && hoverSprite != null)
        {
            buttonImage.sprite = hoverSprite;
            buttonImage.color = Color.white; // Make fully visible
        }
    }
    
    private void HideSprite()
    {
        if (buttonImage != null)
        {
            buttonImage.color = new Color(1f, 1f, 1f, 0f);
        }
    }

    public void PlayHoverSound()
    {
        if (audioPlayer != null)
            audioPlayer.PlayButtonHoverClip();
    }
    
    public void PlayClickSound()
    {
        audioPlayer.PlayButtonClickClip();
    }

    public void PlayBackSound()
    {
        audioPlayer.PlayButtonBackClip();
    }

    public void PlayStartGameSound()
    {
        audioPlayer.PlayButtonStartGameClip();
    }
}
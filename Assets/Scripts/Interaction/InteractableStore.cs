using TMPro;
using UnityEngine;

public class InteractableStore : MonoBehaviour
{
    [SerializeField] GameObject interactablePrompt;
    [SerializeField] GameObject storeMenu;
    
    public bool storeMenuOpened = false;
    
    AudioPlayer audioPlayer;
    bool menuActivated = false;

    void Start()
    {
        audioPlayer = FindFirstObjectByType<AudioPlayer>();
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && menuActivated && !storeMenuOpened)
        {
            DisplayStoreMenu();
            interactablePrompt.gameObject.SetActive(false);
            storeMenuOpened = true;
        }
        else if (Input.GetKeyDown(KeyCode.E) && storeMenuOpened)
        {
            HideStoreMenu();
            interactablePrompt.gameObject.SetActive(true);
            storeMenuOpened = false;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            menuActivated = true;
            interactablePrompt.gameObject.SetActive(true);
        }
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            menuActivated = false;
            interactablePrompt.gameObject.SetActive(false);
        }
    }
    
    void DisplayStoreMenu()
    {
        audioPlayer.PlayInventoryClip();
        Time.timeScale = 0;
        storeMenu.SetActive(true);
    }
    
    void HideStoreMenu()
    {
        Time.timeScale = 1;
        audioPlayer.PlayButtonBackClip();
        storeMenu.SetActive(false);
    }
}

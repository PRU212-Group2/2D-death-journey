using System.Collections;
using TMPro;
using UnityEngine;

public class InteractablePortal : MonoBehaviour
{
    [SerializeField] GameObject interactablePrompt;
    [SerializeField] TextMeshProUGUI keyPrompt;
    [SerializeField] float keyPromptDuration = 3f;
    [SerializeField] ItemSO key;
    [SerializeField] float fadeDuration = 3.5f;
    
    InventoryManager inventoryManager;
    AudioPlayer audioPlayer;
    GameManager gameManager;
    bool doorActivated = false;
    FadeTransition screenTransition;
    
    void Awake()
    {
        audioPlayer = FindFirstObjectByType<AudioPlayer>();
        inventoryManager = FindFirstObjectByType<InventoryManager>();
        gameManager = FindFirstObjectByType<GameManager>();
        screenTransition = gameObject.AddComponent<FadeTransition>();
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && doorActivated)
        {
            interactablePrompt.gameObject.SetActive(false);
            var hasKey = inventoryManager.HasItem(key.itemName);
            if (hasKey)
            {
                audioPlayer.PlayDoorUnlockClip();
                StartCoroutine(LoadNextScene());
            }
            else
            {
                StartCoroutine(ShowKeyPrompt());
            }
        }
    }

    IEnumerator ShowKeyPrompt()
    {
        keyPrompt.gameObject.SetActive(true);
        yield return new WaitForSeconds(keyPromptDuration);
        keyPrompt.gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            doorActivated = true;
            interactablePrompt.gameObject.SetActive(true);
        }
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            doorActivated = false;
            interactablePrompt.gameObject.SetActive(false);
        }
    }
    
    private IEnumerator LoadNextScene()
    {
        yield return StartCoroutine(screenTransition.FadeToBlack(fadeDuration));
        gameManager.MainMenu();
    }
}

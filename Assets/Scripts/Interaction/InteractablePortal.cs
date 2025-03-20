using System.Collections;
using TMPro;
using UnityEngine;

public class InteractablePortal : Interactable
{
    [SerializeField] TextMeshProUGUI keyPrompt;
    [SerializeField] float keyPromptDuration = 3f;
    [SerializeField] ItemSO key;
    [SerializeField] float fadeDuration = 3.5f;
    
    private InventoryManager inventoryManager;
    private GameManager gameManager;
    private FadeTransition screenTransition;
    
    protected override void Start()
    {
        base.Start();
        inventoryManager = FindFirstObjectByType<InventoryManager>();
        gameManager = FindFirstObjectByType<GameManager>();
        screenTransition = gameObject.AddComponent<FadeTransition>();
    }
    
    protected override void OnInteract()
    {
        HidePrompt();
        var hasKey = inventoryManager.HasItem(key.itemName);
        if (hasKey)
        {
            audioPlayer.PlayDoorUnlockClip();
            inventoryManager.FullRemoveItem(key.itemName);
            StartCoroutine(LoadNextScene());
        }
        else
        {
            StartCoroutine(ShowKeyPrompt());
            isInteracting = false;
        }
    }
    
    protected override void OnInteractEnd()
    {
    }

    private IEnumerator ShowKeyPrompt()
    {
        keyPrompt.gameObject.SetActive(true);
        yield return new WaitForSeconds(keyPromptDuration);
        keyPrompt.gameObject.SetActive(false);
    }
    
    private IEnumerator LoadNextScene()
    {
        yield return StartCoroutine(screenTransition.FadeToBlack(fadeDuration));
        gameManager.LoadNextLevel();
    }
}
using System.Collections;
using TMPro;
using UnityEngine;

public class InteractableProps : Interactable
{
    [SerializeField] TextMeshProUGUI propAcquiredText;
    [SerializeField] float propAcquiredDuration = 3f;
    [SerializeField] Item prop;
    
    InventoryManager inventoryManager;
    
    protected override void Start()
    {
        base.Start();
        inventoryManager = FindFirstObjectByType<InventoryManager>();
    }
    
    protected override void OnInteract()
    {
        if(isInteractable)
        {
            audioPlayer.PlayPickupClip();
            HidePrompt();
            StartCoroutine(ShowKeyPrompt());
            inventoryManager.AddItem(prop.itemName, prop.itemQuantity, prop.itemSprite, prop.itemDescription);
            isInteractable = false;
        }
    }
    
    protected override void OnInteractEnd()
    {
    }

    private IEnumerator ShowKeyPrompt()
    {
        propAcquiredText.gameObject.SetActive(true);
        yield return new WaitForSeconds(propAcquiredDuration);
        propAcquiredText.gameObject.SetActive(false);
        ShowPrompt();
    }
}

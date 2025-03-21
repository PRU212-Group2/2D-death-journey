using TMPro;
using UnityEngine;

public class InteractableStore : Interactable
{
    [SerializeField] private GameObject storeMenu;
    
    protected override void OnInteract()
    {
        audioPlayer.PlayInventoryClip();
        Time.timeScale = 0;
        storeMenu.SetActive(true);
        HidePrompt();
    }
    
    protected override void OnInteractEnd()
    {
        Time.timeScale = 1;
        audioPlayer.PlayButtonBackClip();
        storeMenu.SetActive(false);
        ShowPrompt();
    }
}

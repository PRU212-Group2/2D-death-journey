using System.Collections;
using TMPro;
using UnityEngine;

public class InteractableSavePoint : Interactable
{
    [SerializeField] TextMeshProUGUI savedGameText;
    [SerializeField] float savedGameDuration = 1f;
    
    GameManager gameManager;
    
    protected override void Start()
    {
        base.Start();
        gameManager = FindFirstObjectByType<GameManager>();
    }
    
    protected override void OnInteract()
    {
        if(isInteractable)
        {
            audioPlayer.PlaySaveClip();
            HidePrompt();
            StartCoroutine(ShowSavePrompt());
            gameManager.SaveData();
        }
    }
    
    protected override void OnInteractEnd()
    {
    }

    private IEnumerator ShowSavePrompt()
    {
        savedGameText.gameObject.SetActive(true);
        yield return new WaitForSeconds(savedGameDuration);
        savedGameText.gameObject.SetActive(false);
    }
}

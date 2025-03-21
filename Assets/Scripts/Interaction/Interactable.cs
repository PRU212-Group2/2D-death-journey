using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [SerializeField] protected GameObject interactablePrompt;
    
    protected AudioPlayer audioPlayer;
    protected bool isInteractable = true;
    bool isPlayerInRange = false;
    public bool isInteracting = false;
    
    protected virtual void Start()
    {
        audioPlayer = FindFirstObjectByType<AudioPlayer>();
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && isPlayerInRange)
        {
            if (!isInteracting)
            {
                OnInteract();
                isInteracting = true;
            }
            else
            {
                OnInteractEnd();
                isInteracting = false;
            }
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            ShowPrompt();
        }
    }
    
    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            HidePrompt();
            
            if (isInteracting)
            {
                OnInteractEnd();
                isInteracting = false;
            }
        }
    }
    
    protected virtual void ShowPrompt()
    {
        if (isInteractable && interactablePrompt != null)
            interactablePrompt.SetActive(true);
    }
    
    protected virtual void HidePrompt()
    {
        if (interactablePrompt != null)
            interactablePrompt.SetActive(false);
    }
    
    // Abstract methods to be implemented by child classes
    protected abstract void OnInteract();
    protected abstract void OnInteractEnd();
}
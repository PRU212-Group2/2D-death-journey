using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] GameObject InventoryMenu;
    
    bool menuActivated;
    AudioPlayer audioPlayer;

    void Start()
    {
        audioPlayer = FindFirstObjectByType<AudioPlayer>();
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && menuActivated)
        {
            Time.timeScale = 1;
            audioPlayer.PlayButtonBackClip();
            InventoryMenu.SetActive(false);
            menuActivated = false;
        }
        else if (Input.GetKeyDown(KeyCode.Tab) && !menuActivated)
        {
            audioPlayer.PlayInventoryClip();
            Time.timeScale = 0;
            InventoryMenu.SetActive(true);
            menuActivated = true;
        }
    }
}

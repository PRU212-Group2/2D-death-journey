using UnityEngine;
using UnityEngine.SceneManagement;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] GameObject inventoryMenu;
    [SerializeField] public ItemSlot[] itemSlots;
    [SerializeField] ItemSO[] itemSOs;
    
    public bool menuActivated;
    AudioPlayer audioPlayer;

    static InventoryManager _instance;
    
    void Awake()
    {
        ManageSingleton();
    }
    
    // Applying singleton pattern
    void ManageSingleton()
    {
        if (_instance)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    
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
            inventoryMenu.SetActive(false);
            menuActivated = false;
        }
        else if (Input.GetKeyDown(KeyCode.Tab) && !menuActivated)
        {
            audioPlayer.PlayInventoryClip();
            Time.timeScale = 0;
            inventoryMenu.SetActive(true);
            menuActivated = true;
        }
    }

    public int AddItem(string itemName, int quantity, Sprite itemSprite, string itemDescription)
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if ((itemSlots[i].isFull == false 
                && itemSlots[i].itemName == itemName)
                || itemSlots[i].quantity == 0)
            {
                int leftOverItems = itemSlots[i].AddItem(itemName, quantity, itemSprite, itemDescription);
                if (leftOverItems > 0)
                    leftOverItems = AddItem(itemName, leftOverItems, itemSprite, itemDescription);
                    
                return leftOverItems;
            }
        }

        return quantity;
    }
    
    public void RemoveItem(string itemName)
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i].itemName == itemName)
            {
                itemSlots[i].RemoveItem();

                if (itemSlots[i].quantity <= 0)
                {
                    itemSlots[i].EmptySlot();
                }
            }
        }
    }

    public void ClearInventory()
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            itemSlots[i].EmptySlot();
        }
    }

    public bool UseItem(string itemName)
    {
        for (int i = 0; i < itemSOs.Length; i++)
        {
            if (itemSOs[i].itemName == itemName)
            {
                bool usable = itemSOs[i].UseItem();
                return usable;
            }

        }
        return false;
    }

    public bool HasItem(string itemName)
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i].itemName == itemName)
            {
                return true;
            }
        }

        return false;
    }
    
    public void DeselectAllSlots()
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            itemSlots[i].selectedShader.SetActive(false);
            itemSlots[i].itemSelected = false;
        }
    }

    public void PlayItemSound()
    {
        audioPlayer.PlayUseItemClip();
    }
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        var sceneName = SceneManager.GetActiveScene().name;
        // Check if we should start or stop music based on new scene
        if (sceneName == "MainMenu" || sceneName == "Ending")
        {
            Destroy(this.gameObject);
        }
    }
    
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}

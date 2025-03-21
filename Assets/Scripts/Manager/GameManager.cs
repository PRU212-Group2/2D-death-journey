using System;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    static GameManager _instance;
    string _pendingTeleportSceneName = null;
    Vector2 _pendingTeleportPosition = new Vector2(0, 0);

    [Header("Save Settings")] 
    [SerializeField] Item[] items;
    [SerializeField] WeaponSO[] weaponSOs;
    [SerializeField] PlayerSO[] playerSOs;
    
    string saveFilePath;
    ActiveWeapon activeWeapon;
    PlayerMovement playerMovement;
    PlayerHealth playerHealth;
    CurrencyManager currencyManager;
    InventoryManager inventoryManager;

    void Awake()
    {
        ManageSingleton();
        playerMovement = FindFirstObjectByType<PlayerMovement>();
        playerHealth = FindFirstObjectByType<PlayerHealth>();
        currencyManager = FindFirstObjectByType<CurrencyManager>();
        activeWeapon = FindFirstObjectByType<ActiveWeapon>();
        inventoryManager = FindFirstObjectByType<InventoryManager>();
        saveFilePath = Path.Combine(Application.persistentDataPath, "save.json");
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
    
    // Reset game session to the first level
    public void StartNewGame()
    {
        // Delete existing save file if it exists
        if (File.Exists(saveFilePath))
        {
            try
            {
                File.Delete(saveFilePath);
                Debug.Log("Save file deleted successfully.");
            }
            catch (Exception e)
            {
                Debug.LogError("Error deleting save file: " + e.Message);
            }
        }
    
        // Load the first level
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(1);
    }
    
    // Load next level
    public void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        string nextSceneName = GetSceneName(nextSceneIndex);
    
        // Register for scene loaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(nextSceneIndex);
    
        // This stores the name for use in the callback
        _pendingTeleportSceneName = nextSceneName;
    }
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Unregister to prevent multiple calls
        SceneManager.sceneLoaded -= OnSceneLoaded;
    
        // Re-find the player as it might have been recreated in the new scene
        playerMovement = FindFirstObjectByType<PlayerMovement>();
    
        if (playerMovement != null && !string.IsNullOrEmpty(_pendingTeleportSceneName))
        {
            // Teleport player to spawn point after the scene is loaded
            playerMovement.TeleportToSpawnPoint(_pendingTeleportSceneName);
            _pendingTeleportSceneName = null;
        }
    }

    public void ProcessPlayerDeath()
    {
        LoadData();
    }

    string GetSceneName(int sceneIndex)
    {
        string sceneName = SceneUtility.GetScenePathByBuildIndex(sceneIndex);
        sceneName = Path.GetFileNameWithoutExtension(sceneName);
        return sceneName;
    }
    
    // Load Main menu scene
    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    
    // Load Game over scene
    public void QuitGame()
    {
        Application.Quit();
    }
    
    // Load high score from saved file
    public void LoadData()
    {
        if (File.Exists(saveFilePath))
        {
            string jsonData = File.ReadAllText(saveFilePath);
            SaveData data = JsonUtility.FromJson<SaveData>(jsonData);
        
            // Store the position to use after scene is loaded
            _pendingTeleportPosition = new Vector2(data.playerData.position[0], data.playerData.position[1]);
        
            // Register for scene loaded event with our custom callback
            SceneManager.sceneLoaded += OnLoadDataSceneLoaded;
            SceneManager.LoadScene(data.currentLevel);
        }
    }
    
    void OnLoadDataSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Unregister to prevent multiple calls
        SceneManager.sceneLoaded -= OnLoadDataSceneLoaded;
    
        // Re-find references to components in the new scene
        playerMovement = FindFirstObjectByType<PlayerMovement>();
        playerHealth = FindFirstObjectByType<PlayerHealth>();
        currencyManager = FindFirstObjectByType<CurrencyManager>();
        activeWeapon = FindFirstObjectByType<ActiveWeapon>();
        inventoryManager = FindFirstObjectByType<InventoryManager>();
    
        // Only continue if we have valid references
        if (playerMovement != null)
        {
            // Load the save data again
            string jsonData = File.ReadAllText(saveFilePath);
            SaveData data = JsonUtility.FromJson<SaveData>(jsonData);
        
            // Apply the saved data
            currencyManager.LoadCash(data.cash);
            playerHealth.LoadHealth(data.playerData.health);
        
            // Load player
            foreach (PlayerSO player in playerSOs)
            {
                if (player.Name == data.playerData.playerName)
                {
                    playerMovement.SwitchPlayer(player);
                    break;
                }
            }
            
            // Teleport to the saved position
            playerMovement.TeleportToPosition(_pendingTeleportPosition);
        
            // Load weapon
            foreach (WeaponSO weapon in weaponSOs)
            {
                if (weapon.Name == data.weaponName)
                {
                    activeWeapon.SwitchWeapon(weapon);
                    activeWeapon.LoadAmmo(data.ammo);
                    break;
                }
            }
        
            // Load inventory
            inventoryManager.ClearInventory();
            
            var itemsData = data.items;
            foreach (ItemData itemData in itemsData)
            {
                foreach (Item item in items)
                {
                    if (itemData.itemName == item.itemName)
                    {
                        inventoryManager.AddItem(itemData.itemName, itemData.quantity, item.itemSprite, item.itemDescription);
                        break;
                    }
                }
            }
        }
    }
    
    // Save high score to file
    public void SaveData()
    {
        // Re-find references to components in the new scene
        playerMovement = FindFirstObjectByType<PlayerMovement>();
        playerHealth = FindFirstObjectByType<PlayerHealth>();
        currencyManager = FindFirstObjectByType<CurrencyManager>();
        activeWeapon = FindFirstObjectByType<ActiveWeapon>();
        inventoryManager = FindFirstObjectByType<InventoryManager>();
        
        SaveData data = new SaveData();
        data.cash = currencyManager.GetCash();
        data.ammo = activeWeapon.GetAmmo();
        data.weaponName = string.IsNullOrEmpty(activeWeapon.GetCurrentWeapon()) ? "Pistol" : activeWeapon.GetCurrentWeapon();
        
        data.currentLevel = SceneManager.GetActiveScene().buildIndex;
        List<ItemData> tempList = new List<ItemData>();

        foreach (ItemSlot item in inventoryManager.itemSlots)
        {
            tempList.Add(new ItemData
            {
                itemName = item.itemName,
                quantity = item.quantity
            });
        }
            
        ItemData[] itemData = tempList.ToArray();
            
        data.items = itemData;
        data.playerData = new PlayerData()
        {
            position = new[] {playerMovement.transform.position.x, playerMovement.transform.position.y},
            health = playerHealth.GetHealth(),
            playerName = string.IsNullOrEmpty(playerMovement.GetCurrentPlayer()) ? "Adam" : playerMovement.GetCurrentPlayer(),
        };
        
        string jsonData = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFilePath, jsonData);
    }

    public bool HasSave()
    {
        if (File.Exists(saveFilePath))
        {
            // Load the save data again
            string jsonData = File.ReadAllText(saveFilePath);
            SaveData data = JsonUtility.FromJson<SaveData>(jsonData);
            
            if (data.currentLevel > 0) return true;
        }
        return false;
    }
}
using UnityEngine;
using System.IO;

public class CurrencyManager : MonoBehaviour
{
    [SerializeField] int maxCashEarned = 999999;
    
    private int cash;
    
    private string saveFilePath;
    
    static CurrencyManager _instance;
    
    void Awake()
    {
        ManageSingleton();
        // Set the save file path
        saveFilePath = Path.Combine(Application.persistentDataPath, "save.json");
        LoadCash();
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
    
    // Public method to retrieve cash
    public int GetCash()
    {
        return cash;
    }
    
    // Add player cash based on events
    public void AddCash(int amount)
    {
        cash += amount;
        
        // Clamp the cash to not be less than 0
        cash = Mathf.Clamp(cash, 0, maxCashEarned);
    }
    
    // Load cash from saved file
    private void LoadCash()
    {
        if (File.Exists(saveFilePath))
        {
            string jsonData = File.ReadAllText(saveFilePath);
            SaveData data = JsonUtility.FromJson<SaveData>(jsonData);
            cash = data.cash;
        }
    }
    
    // Save high cash to file
    private void SaveCash()
    {
        SaveData data = new SaveData();
        data.cash = cash;
        
        string jsonData = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFilePath, jsonData);
    }
}
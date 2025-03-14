using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Rendering.Universal;

public class ScoreManager : MonoBehaviour
{
    private int score;
    private int highScore;
    
    private string saveFilePath;
    
    static ScoreManager _instance;
    
    void Awake()
    {
        ManageSingleton();
        // Set the save file path
        saveFilePath = Path.Combine(Application.persistentDataPath, "highscore.json");
        LoadHighScore();
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
    
    // Public method to retrieve score
    public int GetScore()
    {
        return score;
    }
    
    // Public method to retrieve high score
    public int GetHighScore()
    {
        return highScore;
    }
    
    // Add player score based on events
    public void AddScore(int amount)
    {
        score += amount;
        
        // Clamp the score to not be less than 0
        score = Mathf.Clamp(score, 0, int.MaxValue);
        
        // Check if current score beats high score
        if (score > highScore)
        {
            highScore = score;
        }
    }
    
    // Reset player score
    public void ResetScore()
    {
        score = 0;
        SaveHighScore();
    }
    
    // Load high score from saved file
    private void LoadHighScore()
    {
        if (File.Exists(saveFilePath))
        {
            string jsonData = File.ReadAllText(saveFilePath);
            SaveData data = JsonUtility.FromJson<SaveData>(jsonData);
            highScore = data.highScore;
        }
        else
        {
            highScore = 0;
        }
    }
    
    // Save high score to file
    private void SaveHighScore()
    {
        SaveData data = new SaveData();
        data.highScore = highScore;
        
        string jsonData = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFilePath, jsonData);
    }
}
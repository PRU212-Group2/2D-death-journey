using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] float loadDelay = 0.5f;

    static GameManager _instance;
    PlayerMovement playerMovement;
    string _pendingTeleportSceneName = null;
    
    void Awake()
    {
        ManageSingleton();
        playerMovement = FindFirstObjectByType<PlayerMovement>();
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
        ResetGame();
    }
    
    void ResetGame()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        string currentSceneName = GetSceneName(currentSceneIndex);
        
        // Register for scene loaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(currentSceneIndex);
        
        // This stores the name for use in the callback
        _pendingTeleportSceneName = currentSceneName;
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
    public void GameOver()
    {
        SceneManager.LoadScene("GameOver");
    }
    
    // Load Game over scene
    public void QuitGame()
    {
        Application.Quit();
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] float loadDelay = 0.5f;

    static GameManager _instance;
    
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
    
    // Switch to game over scene
    public void ProcessPlayerCrash()
    {
        Invoke("GameOver", loadDelay);
    }
    
    // Reset game session to the first level
    public void StartNewGame()
    {
        SceneManager.LoadScene("CityZombie");
    }
    
    // Reset game session to the first level
    public void ResetGame()
    {
        SceneManager.LoadScene("Chapter1");
    }
    
    // Load help menu scene
    public void HelpMenu()
    {
        SceneManager.LoadScene("HelpMenu");
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
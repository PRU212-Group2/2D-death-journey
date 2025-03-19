using System.Collections;
using UnityEngine;

public class EndingMenu : MonoBehaviour
{
    [SerializeField] float fadeDuration = 3.5f;
    
    GameManager gameManager;
    FadeTransition screenTransition;

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        screenTransition = gameObject.AddComponent<FadeTransition>();
    }

    public void StartNewGame()
    {
        StartCoroutine(StartGameWithFade());
    }

    public void MainMenu()
    {
        gameManager.MainMenu();
    }
    
    public void ExitGame()
    {
        gameManager.QuitGame();
    }

    private IEnumerator StartGameWithFade()
    {
        yield return StartCoroutine(screenTransition.FadeToBlack(fadeDuration));
        gameManager.StartNewGame();
    }
}

using System.Collections;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    GameManager gameManager;
    [SerializeField] float fadeDuration = 3.5f;
    private FadeTransition screenTransition;

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        screenTransition = gameObject.AddComponent<FadeTransition>();
    }

    public void StartNewGame()
    {
        StartCoroutine(StartGameWithFade());
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
using System.Collections;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] float fadeDuration = 3.5f;
    [SerializeField] Button ContinueButton;
    
    GameManager gameManager;
    FadeTransition screenTransition;

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        screenTransition = gameObject.AddComponent<FadeTransition>();
        EnableContinueButton();
    }

    public void ContinueGame()
    {
        StartCoroutine(ContinueGameWithFade());

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
    
    private IEnumerator ContinueGameWithFade()
    {
        yield return StartCoroutine(screenTransition.FadeToBlack(fadeDuration));
        gameManager.LoadData();
    }

    private void EnableContinueButton()
    {
        if (gameManager.HasSave())
        {
            ContinueButton.gameObject.SetActive(true);
        }
    }
}
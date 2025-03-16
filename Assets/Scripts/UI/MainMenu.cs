using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    GameManager gameManager;
    [SerializeField] float fadeDuration = 3.5f;
    private float alpha = 0f;
    private bool isFading = false;

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
    }

    public void StartNewGame()
    {
        StartCoroutine(FadeAndStartGame());
    }

    public void ExitGame()
    {
        gameManager.QuitGame();
    }

    private IEnumerator FadeAndStartGame()
    {
        isFading = true; // Start fading to black
        float startAlpha = 0f;
        float endAlpha = 1f;

        for (float t = 0f; t < fadeDuration; t += Time.deltaTime)
        {
            float normalizedTime = t / fadeDuration;
            alpha = Mathf.Lerp(startAlpha, endAlpha, normalizedTime);
            yield return null;
        }

        // Ensure it's fully black at the end
        alpha = 1f;

        // Start the new game after the fade
        gameManager.StartNewGame();
    }

    void OnGUI()
    {
        if (isFading)
        {
            // Set the color to black and apply alpha
            Color blackColor = new Color(0, 0, 0, alpha);
            GUI.color = blackColor;
            
            // Draw a fullscreen box that fades to black
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
        }
    }
}
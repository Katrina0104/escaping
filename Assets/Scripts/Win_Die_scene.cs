using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinDieScene : MonoBehaviour
{
    [Header("Fade Settings")]
    public Image fadeImage;              // UI Image (black), stretched to fill screen
    public float fadeDuration = 2f;      // Duration of fade in seconds

    [Header("Scene Names")]
    public string loseSceneName = "GameOver";
    public string winSceneName = "Win";

    private bool isEnding = false;
    private bool playerLost = false;
    private float fadeTimer = 0f;

    void Start()
    {
        // Start fully transparent
        if (fadeImage != null)
        {
            fadeImage.color = new Color(0f, 0f, 0f, 0f);
        }
    }

    void Update()
    {
        // Check lose condition
        bool playerDead = PlayerHealth.Instance != null && PlayerHealth.Instance.IsDead();

        // Check win condition
        bool enemyWin = EnemyHealth.Instance != null && EnemyHealth.EnemyDeath >= 10;

        // If either condition met and we haven't started ending yet
        if (!isEnding && (playerDead || enemyWin))
        {
            isEnding = true;
            playerLost = playerDead;
            fadeTimer = 0f;
        }

        // Handle fade if game is ending
        if (isEnding)
        {
            fadeTimer += Time.deltaTime;
            float alpha = Mathf.Clamp01(fadeTimer / fadeDuration);

            if (fadeImage != null)
            {
                fadeImage.color = new Color(0f, 0f, 0f, alpha); // Fade to black
            }

            // Once fade is complete, load the appropriate scene
            if (fadeTimer >= fadeDuration)
            {
                string targetScene = playerLost ? loseSceneName : winSceneName;
                SceneManager.LoadScene(targetScene);
            }
        }
    }
}

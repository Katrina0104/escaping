using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Win_Die_scene : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 2f;
    public string loseSceneName = "GameOver";
    public string winSceneName = "Win";

    private bool isEnding = false;
    private bool playerLost = false;
    private float fadeTimer = 0f;

    void Update()
    {
        bool playerDead = PlayerHealth.Instance != null && PlayerHealth.Instance.IsDead();
        bool enemyWin = EnemyHealth.Instance != null && EnemyHealth.EnemyDeath >= 20;

        if (!isEnding && (playerDead || enemyWin))
        {
            isEnding = true;
            playerLost = playerDead;
            fadeTimer = 0f;
        }

        if (isEnding)
        {
            fadeTimer += Time.deltaTime;
            float alpha = Mathf.Clamp01(fadeTimer / fadeDuration);
            if (fadeImage != null)
            {
                fadeImage.color = new Color(0, 0, 0, alpha);
            }

            if (fadeTimer >= fadeDuration)
            {
                string targetScene = playerLost ? loseSceneName : winSceneName;
                SceneManager.LoadScene(targetScene);
            }
        }
    }
}

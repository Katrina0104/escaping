using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class FadeAndClickToSwitch : MonoBehaviour
{
    public CanvasGroup textCanvasGroup;
    public float fadeDuration = 2f;
    public float waitBeforeClick = 2f;
    public string sceneToLoad = "Main_Map";

    private bool canClick = false;

    void Start()
    {
        if (textCanvasGroup != null)
        {
            textCanvasGroup.alpha = 0f;
            StartCoroutine(FadeIn());
        }
    }

    IEnumerator FadeIn()
    {
        float timer = 0f;

        // Wait before starting fade
        yield return new WaitForSeconds(0.5f);

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            textCanvasGroup.alpha = Mathf.Clamp01(timer / fadeDuration);
            yield return null;
        }

        // Wait before allowing click
        yield return new WaitForSeconds(waitBeforeClick);
        canClick = true;
    }

    void Update()
    {
        if (canClick && Input.GetMouseButtonDown(0))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}

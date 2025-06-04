using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
     
    // Set this in the Inspector to the name of the scene you want to load
    public string targetSceneName = "Main_Map";

    public void SwitchScene()
    {
        SceneManager.LoadScene(targetSceneName);
    }
}

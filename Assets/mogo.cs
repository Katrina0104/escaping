using UnityEngine;

public class DetectFpsControllerTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "FpsController")
        {
            //Debug.Log("Triggered by FpsController!");
        }
    }
}

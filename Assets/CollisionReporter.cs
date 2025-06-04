using UnityEngine;

public class CollisionReporter : MonoBehaviour
{
    private CollisionManager manager;

    void Start()
    {
        // Auto-find GameObject named "EventSystem" and get the CollisionManager component
        GameObject managerObject = GameObject.Find("EventSystem");
        if (managerObject != null)
        {
            manager = managerObject.GetComponent<CollisionManager>();
        }

        if (manager == null)
        {
            Debug.LogWarning("CollisionManager not found on 'EventSystem'.");
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (manager != null)
        {
            manager.ReportCollision(gameObject, collision.gameObject);
        }
    }
}

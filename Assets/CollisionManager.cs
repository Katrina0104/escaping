using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    public void ReportCollision(GameObject objA, GameObject objB)
    {
        Debug.Log($"[Manager] Collision detected between {objA.name} and {objB.name}");
    }
}

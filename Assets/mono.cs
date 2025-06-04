using UnityEngine;

public class SimpleAnimationLogger : MonoBehaviour
{
    public Animator animator;

    private bool hasLogged = false;

    void Update()
    {
        if (animator == null) return;

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("DEMON ATTACK"))
        {
            // Check if we haven't logged it yet
            if (!hasLogged)
            {
                Debug.Log("Animation started: DEMON ATTACK");
                hasLogged = true;
            }
        }
        else
        {
            // Reset flag when animation is NOT playing
            hasLogged = false;
        }
    }
}

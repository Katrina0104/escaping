using UnityEngine;
using System;
using System.Collections;

public class DelayedDoubleCheckLogger : MonoBehaviour
{
    public Animator animator;
    public float delayBeforeHit = 0.5f; // ⏱ Tweakable delay in seconds

    public static event Action PlayerLostHP;

    private bool isCollidingWithPlayer = false;
    private bool hasTriggeredThisAttack = false;
    private int attackCounter = 0;

    void Update()
    {
        if (animator == null) return;

        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        bool isAttacking = stateInfo.IsName("DEMON ATTACK");

        if (isAttacking && !hasTriggeredThisAttack)
        {
            hasTriggeredThisAttack = true;
            StartCoroutine(DelayedCheck());
        }
        else if (!isAttacking)
        {
            hasTriggeredThisAttack = false;
        }
    }

    IEnumerator DelayedCheck()
    {
        yield return new WaitForSeconds(delayBeforeHit);

        if (isCollidingWithPlayer)
        {
            attackCounter++;
            Debug.Log("DEMON ATTACK hit confirmed! Count: " + attackCounter);
            PlayerLostHP?.Invoke();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "FpsController")
        {
            isCollidingWithPlayer = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "FpsController")
        {
            isCollidingWithPlayer = false;
        }
    }
}

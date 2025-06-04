using UnityEngine;

[RequireComponent(typeof(AdvancedEnemyAI))]
public class EnemyAIStateTracker : MonoBehaviour
{
    public Animator animator;

    private AdvancedEnemyAI enemyAI;

    void Awake()
    {
        enemyAI = GetComponent<AdvancedEnemyAI>();

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    void Update()
    {
        switch (enemyAI.currentState)
        {
            case AdvancedEnemyAI.AIState.Wander:
                // Only set isWandering true if not idle
                animator.SetBool("isWandering", !enemyAI.IsIdle);
                animator.SetBool("isIdle", enemyAI.IsIdle);

                animator.SetBool("isChasing", false);
                animator.SetBool("isAttacking", false);
                break;

            case AdvancedEnemyAI.AIState.Chase:
                animator.SetBool("isWandering", false);
                animator.SetBool("isIdle", false);
                animator.SetBool("isChasing", true);
                animator.SetBool("isAttacking", false);
                break;

            case AdvancedEnemyAI.AIState.Attack:
                animator.SetBool("isWandering", false);
                animator.SetBool("isIdle", false);
                animator.SetBool("isChasing", false);
                animator.SetBool("isAttacking", true);
                break;
        }


    }
}

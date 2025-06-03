using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(NavMeshAgent))]
public class AdvancedEnemyAI : MonoBehaviour
{
    public enum AIState { Wander, Chase, Attack }

    [Header("State Settings")]
    public AIState currentState = AIState.Wander;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float chaseRange = 10f;
    [SerializeField] private float attackDuration = 1.5f;

    [Header("Detection Settings")]
    public float detectionRadius = 12f;
    public LayerMask playerLayer;

    [Header("Wander Settings")]
    public float wanderRadius = 10f;
    public float minWaitTime = 1f;
    public float maxWaitTime = 3f;
    [SerializeField] private float homePullStrength = 0.7f;

    [Header("Movement Settings")]
    public float wanderSpeed = 2f;
    public float chaseSpeed = 5f;

    [Header("References")]
    public Transform target;
    public UnityEvent OnAttack;

    private NavMeshAgent agent;
    private float stateTimer;
    private Vector3 homePosition;
    private bool isAttacking;
    private Vector3 currentDestination;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        homePosition = transform.position;
    }

    void Update()
    {
        switch (currentState)
        {
            case AIState.Wander:
                UpdateWanderState();
                break;
            case AIState.Chase:
                UpdateChaseState();
                break;
            case AIState.Attack:
                UpdateAttackState();
                break;
        }
    }

    void UpdateWanderState()
    {
        // Check for player nearby using CheckSphere
        if (Physics.CheckSphere(transform.position, detectionRadius, playerLayer))
        {
            currentState = AIState.Chase;
            return;
        }

        if (!isAttacking && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (stateTimer <= 0)
            {
                SetRandomDestination();
                stateTimer = Random.Range(minWaitTime, maxWaitTime);
            }
            else
            {
                stateTimer -= Time.deltaTime;
            }
        }
    }

    void UpdateChaseState()
    {
        if (target == null)
        {
            currentState = AIState.Wander;
            return;
        }

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance <= attackRange)
        {
            currentState = AIState.Attack;
            StartAttack();
            return;
        }

        if (distance > chaseRange * 1.5f)
        {
            currentState = AIState.Wander;
            return;
        }

        agent.speed = chaseSpeed;
        agent.SetDestination(target.position);
    }

    void UpdateAttackState()
    {
        if (!isAttacking)
        {
            if (target != null && Vector3.Distance(transform.position, target.position) > attackRange * 1.2f)
            {
                currentState = AIState.Chase;
            }
            else
            {
                currentState = AIState.Wander;
            }
        }
    }

    void StartAttack()
    {
        isAttacking = true;
        agent.isStopped = true;
        OnAttack.Invoke();
        Invoke("FinishAttack", attackDuration);
    }

    void FinishAttack()
    {
        isAttacking = false;
        agent.isStopped = false;
    }

    void SetRandomDestination()
    {
        Vector3 randomDirection = GetBoundedRandomDirection();

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, wanderRadius, NavMesh.AllAreas))
        {
            currentDestination = hit.position;
            agent.speed = wanderSpeed;
            agent.SetDestination(currentDestination);
        }
    }

    Vector3 GetBoundedRandomDirection()
    {
        Vector2 randomCircle = Random.insideUnitCircle * wanderRadius;
        Vector3 randomPoint = new Vector3(
            homePosition.x + randomCircle.x,
            homePosition.y,
            homePosition.z + randomCircle.y
        );

        Vector3 pullDirection = (homePosition - transform.position).normalized;
        float distanceFromHome = Vector3.Distance(
            new Vector3(transform.position.x, 0, transform.position.z),
            new Vector3(homePosition.x, 0, homePosition.z)
        );

        float pullFactor = Mathf.Clamp01(distanceFromHome / wanderRadius) * homePullStrength;

        return Vector3.Lerp(randomPoint, homePosition, pullFactor);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 1, 0.3f);
        Gizmos.DrawWireSphere(homePosition, wanderRadius);

        Gizmos.color = new Color(1, 1, 0, 0.2f);
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, detectionRadius); // Visualize CheckSphere

        if (agent != null && agent.hasPath)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, currentDestination);
            Gizmos.DrawWireCube(currentDestination, Vector3.one * 0.5f);
        }

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(homePosition, 0.25f);
    }

    public void SetHomePosition(Vector3 newHome)
    {
        homePosition = newHome;
    }

    public void SetState(AIState newState)
    {
        currentState = newState;
        stateTimer = 0;
    }
}

using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class HomeBoundWanderer : MonoBehaviour
{
    [Header("Home Position Settings")]
    [Tooltip("Center point the AI should stay near")]
    public Vector3 homePosition = Vector3.zero;

    [Tooltip("Maximum distance from home position (XZ plane)")]
    public float maxRoamDistance = 10f;

    [Tooltip("How strongly the AI is pulled back toward home (0 = no pull, 1 = strict boundary)")]
    [Range(0f, 1f)] public float homePullStrength = 0.5f;

    [Header("Wandering Settings")]
    public float minWaitTime = 1f;
    public float maxWaitTime = 3f;
    public float moveSpeed = 3f;
    public float turnSpeed = 120f;

    [Header("Debug")]
    [SerializeField] private Vector3 currentDestination;
    [SerializeField] private float waitTimer;
    [SerializeField] private bool isWaiting;

    private NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        agent.angularSpeed = turnSpeed;
        homePosition = transform.position; // Default to starting position
        SetNewDestination();
    }

    private void Update()
    {
        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0)
            {
                isWaiting = false;
                SetNewDestination();
            }
            return;
        }

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            StartWaiting();
        }
    }

    private void SetNewDestination()
    {
        Vector3 randomDirection = GetBoundedRandomDirection();

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, maxRoamDistance, NavMesh.AllAreas))
        {
            currentDestination = hit.position;
            agent.SetDestination(currentDestination);
        }
    }

    private Vector3 GetBoundedRandomDirection()
    {
        // Base random point
        Vector2 randomCircle = Random.insideUnitCircle * maxRoamDistance;
        Vector3 randomPoint = new Vector3(
            homePosition.x + randomCircle.x,
            homePosition.y,
            homePosition.z + randomCircle.y
        );

        // Apply pull toward home
        Vector3 pullDirection = (homePosition - transform.position).normalized;
        float distanceFromHome = Vector3.Distance(
            new Vector3(transform.position.x, 0, transform.position.z),
            new Vector3(homePosition.x, 0, homePosition.z)
        );

        // Normalized pull factor (0 at center, 1 at boundary)
        float pullFactor = Mathf.Clamp01(distanceFromHome / maxRoamDistance) * homePullStrength;

        return Vector3.Lerp(randomPoint, homePosition, pullFactor);
    }

    private void StartWaiting()
    {
        isWaiting = true;
        waitTimer = Random.Range(minWaitTime, maxWaitTime);
    }

    // Call this to dynamically update home position
    public void SetHomePosition(Vector3 newHome)
    {
        homePosition = newHome;
    }

    private void OnDrawGizmosSelected()
    {
        // Draw home boundary
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(homePosition, maxRoamDistance);

        // Draw current path
        if (agent != null && agent.hasPath)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, currentDestination);
            Gizmos.DrawWireCube(currentDestination, Vector3.one);
        }

        // Draw home position marker
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(homePosition, 0.5f);
    }
}
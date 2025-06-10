using UnityEngine;
using UnityEngine.AI;

public class NPC : MonoBehaviour
{
    public float wanderRadius = 10f;
    public float wanderInterval = 5f;

    private NavMeshAgent agent;
    private Animator animator;
    private float timer;
    private bool isDead = false;

    // New: Flag to mark if NPC caught the player stealing
    public bool playerCaughtStealing = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        timer = wanderInterval;
    }

    void Update()
    {
        if (isDead) return;

        timer += Time.deltaTime;

        if (timer >= wanderInterval)
        {
            Vector3 newDestination = GetRandomDestination();
            agent.SetDestination(newDestination);
            timer = 0;
        }

        if (animator != null)
        {
            float speed = agent.velocity.magnitude;
            animator.SetFloat("Speed", speed);
        }
    }

    Vector3 GetRandomDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, NavMesh.AllAreas))
        {
            return hit.position;
        }

        return transform.position;
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;

        if (animator != null)
        {
            animator.SetBool("isDead", true);
        }

        if (agent != null)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }

        Debug.Log("NPC is dead.");
    }

    // New: Detect player stealing if inside trigger collider
    void OnTriggerStay(Collider other)
    {
        if (isDead) return;

        if (other.CompareTag("Player"))
        {
                
            PlayerBehaviour player = other.GetComponent<PlayerBehaviour>();

            if (player != null && player.isStealing && SunRotates.Instance != null)
            {
                float currentTime = SunRotates.Instance.GetCurrentTime();

                if (currentTime >= 6f && currentTime < 12f) // morning check
                {
                    if (!playerCaughtStealing)
                    {
                        playerCaughtStealing = true;
                        Debug.Log($"{gameObject.name} caught the player stealing in the morning!");
                        // You can add additional logic here, e.g., alerting a manager or UI
                    }
                }
            }
        }
    }
}

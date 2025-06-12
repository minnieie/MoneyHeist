using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;


public class NPC : MonoBehaviour
{
    public float wanderRadius = 10f;
    public float wanderInterval = 5f;
    private List<GameObject> droppedItems = new List<GameObject>();

    [Header("Item Drop Settings")]
    public GameObject[] possibleDrops; // Assign in inspector
    [Range(0f, 1f)] public float dropChance = 0.5f; // 50% chance to drop
    public Vector3 dropOffset = new Vector3(0, 0.1f, 0); // Small vertical offset

    [Header("Alive Drop Settings")]
    [Range(0f, 1f)] public float aliveDropChance = 0.1f; // Chance to drop while alive
    public float aliveDropInterval = 10f; // Time between drop checks
    public float minDistanceForDrop = 2f; // Minimum distance moved to consider dropping

    private NavMeshAgent agent;
    private Animator animator;
    private float wanderTimer;
    private float dropTimer;
    private bool isDead = false;
    public bool playerCaughtStealing = false;
    private Vector3 lastPosition;
    private float distanceMoved;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        wanderTimer = wanderInterval;
        dropTimer = aliveDropInterval;
        lastPosition = transform.position;
    }

    void Update()
    {
        if (isDead) return;

        // Update timers
        wanderTimer += Time.deltaTime;
        dropTimer += Time.deltaTime;

        // Calculate distance moved since last frame
        distanceMoved += Vector3.Distance(transform.position, lastPosition);
        lastPosition = transform.position;

        // Handle wandering
        if (wanderTimer >= wanderInterval)
        {
            Vector3 newDestination = GetRandomDestination();
            agent.SetDestination(newDestination);
            wanderTimer = 0;
        }

        // Handle random drops while alive
        if (dropTimer >= aliveDropInterval)
        {
            // Only consider dropping if NPC has moved enough
            if (distanceMoved >= minDistanceForDrop)
            {
                TryAliveDrop();
            }
            dropTimer = 0;
            distanceMoved = 0;
        }

        // Update animator
        if (animator != null)
        {
            float speed = agent.velocity.magnitude;
            animator.SetFloat("Speed", speed);
        }
    }

    void TryAliveDrop()
    {
        if (possibleDrops.Length == 0) return;

        if (Random.value <= aliveDropChance)
        {
            GameObject itemToDrop = possibleDrops[Random.Range(0, possibleDrops.Length)];
            SpawnItem(itemToDrop);
            //Debug.Log("NPC dropped an item while wandering!");
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

        // Disable colliders
        Collider[] colliders = GetComponents<Collider>();
        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }

        // Drop item
        TryDropItem();

        Debug.Log("NPC is dead.");
    }

    void TryDropItem()
    {
        if (possibleDrops.Length == 0) return;

        if (Random.value <= dropChance)
        {
            GameObject itemToDrop = possibleDrops[Random.Range(0, possibleDrops.Length)];
            SpawnItem(itemToDrop);
        }
    }

    void SpawnItem(GameObject itemToDrop)
    {
        // Calculate spawn position with offset
        Vector3 spawnPosition = transform.position + dropOffset;

        // Raycast to find ground position
        if (Physics.Raycast(spawnPosition + Vector3.up * 2f, Vector3.down, out RaycastHit hit, 5f))
        {
            spawnPosition = hit.point;
            spawnPosition += new Vector3(0, 0.05f, 0); // Tiny offset to prevent z-fighting
        }

        // Track the dropped item
        GameObject droppedItem = Instantiate(itemToDrop, spawnPosition, Quaternion.identity);
        droppedItems.Add(droppedItem);
    }

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
                    }
                }
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("Projectile"))
        {
            Die();
            Destroy(collision.gameObject);
        }
    }
    public void ResetNPC()
    {   
        // Reset death state
        isDead = false;

        // Clear dropped items
        foreach (GameObject item in droppedItems)
        {
            if (item != null)
            {
                Destroy(item);
            }
        }

        droppedItems.Clear();
        // Reset animator
        if (animator != null)
        {
            animator.SetBool("isDead", false);
            animator.SetFloat("Speed", 0f);
        }

        // Re-enable NavMeshAgent
        if (agent != null)
        {
            agent.enabled = true;
            agent.isStopped = false;
            agent.ResetPath();
        }

        // Unfreeze Rigidbody constraints
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.None;
        }

        // Re-enable all colliders
        Collider[] colliders = GetComponents<Collider>();
        foreach (Collider col in colliders)
        {
            col.enabled = true;
        }

        // Reset timers
        wanderTimer = wanderInterval;
        dropTimer = aliveDropInterval;
        distanceMoved = 0f;
        lastPosition = transform.position;

        // Reset playerCaughtStealing flag
        playerCaughtStealing = false;
    }

}

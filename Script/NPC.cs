using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class NPC : MonoBehaviour
{
    [Header("Wandering Settings")]
    public float wanderRadius = 10f;
    public float wanderInterval = 5f;

    [Header("Item Drop Settings")]
    public GameObject[] possibleDrops;
    [Range(0f, 1f)] public float dropChance = 0.5f;
    public Vector3 dropOffset = new Vector3(0, 0.1f, 0);

    [Header("Drop Limits")]
    public int maxDropsAlive = 3;
    public int maxDropsTotal = 5;
    public float minDropDistance = 2f;

    private NavMeshAgent agent;
    private Animator animator;
    private float wanderTimer;
    private bool isDead = false;
    private List<GameObject> droppedItems = new List<GameObject>();
    private Vector3 lastPosition;
    private float distanceMoved;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        wanderTimer = wanderInterval;
        lastPosition = transform.position;
    }

    void Update()
    {
        if (isDead) return;

        wanderTimer += Time.deltaTime;
        distanceMoved += Vector3.Distance(transform.position, lastPosition);
        lastPosition = transform.position;

        if (wanderTimer >= wanderInterval)
        {
            Wander();
            wanderTimer = 0;

            CollectibleManager manager = FindAnyObjectByType<CollectibleManager>();
            if (distanceMoved >= minDropDistance &&
                droppedItems.Count < maxDropsAlive &&
                manager != null)
            {
                TryDrop();
                distanceMoved = 0;
            }
        }

        UpdateAnimator();
    }

    void Wander()
    {
        Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
        agent.SetDestination(newPos);
    }

    void OnTriggerEnter(Collider other)
    {
        if (isDead) return;

        if (other.CompareTag("Projectile"))
        {
            Die();
            Destroy(other.gameObject);
        }
    }

    void TryDrop()
    {
        if (possibleDrops.Length == 0 || Random.value > dropChance)
            return;

        GameObject item = possibleDrops[Random.Range(0, possibleDrops.Length)];
        SpawnItem(item);
    }

    void SpawnItem(GameObject itemPrefab)
    {
        Vector3 spawnPos = transform.position + dropOffset;

        if (Physics.Raycast(spawnPos + Vector3.up * 2f, Vector3.down, out RaycastHit hit, 5f))
        {
            spawnPos = hit.point + Vector3.up * 0.05f;
        }

        GameObject newItem = Instantiate(itemPrefab, spawnPos, Quaternion.identity);
        droppedItems.Add(newItem);
    }

    public void ItemCollected(GameObject item)
    {
        if (droppedItems.Contains(item))
        {
            droppedItems.Remove(item);
        }
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;
        if (animator != null) animator.SetBool("IsDead", true);
        if (agent != null) agent.isStopped = true;

        CollectibleManager manager = FindAnyObjectByType<CollectibleManager>();
        if (manager != null &&
            manager.totalCollectibles < manager.maxCollectiblesAllowed &&
            droppedItems.Count < maxDropsTotal)
        {
            TryDrop();
        }
    }

    void UpdateAnimator()
    {
        if (animator != null)
        {
            animator.SetFloat("Speed", agent.velocity.magnitude);
        }
    }

    Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;

        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }

    public void ResetNPC()
    {
        isDead = false;
        if (animator != null) animator.SetBool("IsDead", false);
        if (agent != null) agent.isStopped = false;
        CleanDroppedItems();
    }

    public void CleanDroppedItems()
    {
        foreach (GameObject item in droppedItems)
        {
            if (item != null)
                Destroy(item);
        }
        droppedItems.Clear();
    }
}

}

using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    int score = 5;
    int currentHealth = 10;
    int maxHealth = 10;
    int CoinScore = 0;
    bool canInteract = false;

    [SerializeField]
    float interactionDistance = 0.5f;

    [SerializeField]
    public GameObject projectile;

    [SerializeField]
    Transform spawnPoint;

    [SerializeField]
    float fireStrength = 0f;

    CoinBehaviour currentCoin = null;
    DoorBehaviour currentDoor = null;

    // Added for hazard damage cooldown control
    private float hazardDamageCooldown = 1f; // seconds between damage ticks
    private float lastHazardDamageTime = 0f;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Collectible"))
        {
            score++;
            Debug.Log("Score: " + score);
        }
        if (collision.gameObject.CompareTag("HealingArea"))
        {
            if (currentHealth < maxHealth)
            {
                ++currentHealth;
                Debug.Log("Current Health: " + currentHealth);
            }
            else
            {
                Debug.Log("Health is already full");
            }
        }
        if (collision.gameObject.CompareTag("HazardArea"))
        {
            currentHealth--;
            Debug.Log("Health: " + currentHealth);
            if (currentHealth <= 0)
            {
                Debug.Log("Player is dead");
            }
        }
    }

    // Added: handle trigger-based hazard damage
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HazardArea"))
        {
            ApplyHazardDamage();
            lastHazardDamageTime = Time.time;
        }

        if (other.gameObject.CompareTag("Collectible"))
        {
            Debug.Log("Player is looking at " + other.gameObject.name);
            currentCoin = other.gameObject.GetComponent<CoinBehaviour>();
            canInteract = true;
            if (currentCoin != null)
            {
                currentCoin.Highlight();
            }
        }
        else if (other.CompareTag("Door"))
        {
            canInteract = true;
            currentDoor = other.GetComponent<DoorBehaviour>();
        }
    }

    // Added: repeatedly damage player while inside hazard trigger
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("HazardArea"))
        {
            if (Time.time - lastHazardDamageTime >= hazardDamageCooldown)
            {
                ApplyHazardDamage();
                lastHazardDamageTime = Time.time;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Collectible"))
        {
            if (currentCoin != null)
            {
                currentCoin.UnHighlight();
            }
            currentCoin = null;
            canInteract = false;
        }
    }

    void OnInteract()
    {
        if (canInteract)
        {
            if (currentCoin != null)
            {
                Debug.Log("Interacting with coin");
                currentCoin.Collect(this);
                currentCoin.UnHighlight();
                currentCoin = null;
                canInteract = false;
            }
            else if (currentDoor != null)
            {
                Debug.Log("Interacting with door");
                currentDoor.interact();
            }
        }
    }

    public void ModifyScore(int amount)
    {
        CoinScore += amount;
        Debug.Log("Score: " + CoinScore);
    }

    void OnFire()
    {
        GameObject newProjectile = Instantiate(projectile, spawnPoint.position, spawnPoint.rotation);
        Vector3 fireForce = spawnPoint.forward * fireStrength;
        newProjectile.GetComponent<Rigidbody>().AddForce(fireForce);
    }

    void Update()
    {
        RaycastHit[] hits = Physics.RaycastAll(spawnPoint.position, spawnPoint.forward, interactionDistance);
        Debug.DrawRay(spawnPoint.position, spawnPoint.forward * interactionDistance, Color.green);

        if (hits.Length > 0)
        {
            RaycastHit closestHit = hits[0]; // Assume first hit is closest
            float closestDistance = Vector3.Distance(transform.position, closestHit.collider.transform.position);

            foreach (RaycastHit hit in hits)
            {
                float distance = Vector3.Distance(transform.position, hit.collider.transform.position);
                if (distance < closestDistance)
                {
                    closestHit = hit;
                    closestDistance = distance;
                }
            }

            if (closestHit.collider.gameObject.CompareTag("Collectible"))
            {
                if (currentCoin != null && currentCoin.gameObject != closestHit.collider.gameObject)
                {
                    currentCoin.UnHighlight();
                }

                currentCoin = closestHit.collider.gameObject.GetComponent<CoinBehaviour>();
                if (currentCoin != null)
                {
                    currentCoin.Highlight();
                    canInteract = true;
                }
            }
        }
        else if (currentCoin != null) // If nothing is detected, unhighlight
        {
            currentCoin.UnHighlight();
            currentCoin = null;
            canInteract = false;
        }
    }

    public void MarkAsThief()
    {
        Debug.Log("You have committed theft!");
    }

    // Helper method for damage application
    void ApplyHazardDamage()
    {
        currentHealth--;
        Debug.Log("Health: " + currentHealth);
        if (currentHealth <= 0)
        {
            Debug.Log("Player is dead");
            // Handle death here if needed
        }
    }
}

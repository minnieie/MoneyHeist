using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    public int score = 0;
    public int currentHealth = 10;
    public int maxHealth = 10;
    public int coinScore = 0;
    public bool canInteract = false;
    public bool isStealing = false;
    public float stealTimer = 0f;

    public bool isDead = false; // <== ADD THIS


    [SerializeField] private float interactionDistance = 1.5f;
    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float fireStrength = 0f;

    private float hazardDamageCooldown = 1f;
    private float lastHazardDamageTime = 0f;

    private float stealResetTimer = 3f;

    [SerializeField]
    private PanelToggle panelToggle;

    public CoinBehaviour currentCoin = null;

    [SerializeField]
    public Transform playerSpawnPoint;

    private void RespawnPlayer()
    {
        if (playerSpawnPoint != null)
        {   
            transform.position = playerSpawnPoint.position;
            ResetPlayer();
            Debug.Log("Player has respawned!");
        }
    }

    public void ResetPlayer()
    {
        Debug.Log("ResetPlayer called");
        currentHealth = maxHealth;
        coinScore = 0;
        score = 0;
        canInteract = false;
        isStealing = false;
        stealTimer = 0f;
        currentCoin = null;
        isDead = false; // ✅ Allow damage again

        panelToggle?.UpdateScoreDisplay(0);
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Collectible"))
        {
            score++;
            Debug.Log("Score: " + score);
        }

        // Remove hazard damage here to avoid double damage

        if (collision.gameObject.CompareTag("HealingArea") && currentHealth < maxHealth)
        {
            currentHealth = Mathf.Min(currentHealth + 1, maxHealth);
            Debug.Log("Current Health: " + currentHealth);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HazardArea"))
        {
            ApplyHazardDamage();
            lastHazardDamageTime = Time.time;

            Renderer hazardRenderer = other.GetComponent<Renderer>();
            if (hazardRenderer != null)
                hazardRenderer.material.color = Color.red;
        }

        if (other.CompareTag("Collectible"))
        {
            currentCoin = other.GetComponent<CoinBehaviour>();
            canInteract = currentCoin != null;
            currentCoin?.Highlight();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("HazardArea") &&
            Time.time - lastHazardDamageTime >= hazardDamageCooldown)
        {
            ApplyHazardDamage();
            lastHazardDamageTime = Time.time;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Collectible") && currentCoin != null)
        {
            currentCoin.UnHighlight();
            currentCoin = null;
            canInteract = false;
        }
    }

    private void OnInteract()
    {
        if (!canInteract || currentCoin == null) return;

        if (currentCoin.isStolenItem && SunRotates.Instance != null && !SunRotates.Instance.IsNight)
        {
            isStealing = true;
            stealTimer = 0f;

            if (panelToggle != null && !panelToggle.isTheftTimerRunning)
                panelToggle.StartTheftTimer();
        }
        else
        {
            isStealing = false;
        }

        currentCoin.Collect(this);
        currentCoin.UnHighlight();
        currentCoin = null;
        canInteract = false;
    }

    public void ModifyScore(int amount)
    {
        coinScore += amount;
        panelToggle?.UpdateScoreDisplay(coinScore);

        if (panelToggle != null && panelToggle.isTheftTimerRunning && coinScore >= 100)
        {
            panelToggle.StopTheftTimer();
            Debug.Log("Successfully stole enough items!");
        }
    }

    private void OnFire()
    {
        GameObject newProjectile = Instantiate(projectile, spawnPoint.position, spawnPoint.rotation);
        newProjectile.GetComponent<Rigidbody>().AddForce(spawnPoint.forward * fireStrength);
    }

    private void Update()
    {
        HandleStealing();
        HandleInteractionInput();
        HandleRaycastHighlight();
    }

    private void HandleStealing()
    {
        if (isStealing)
        {
            stealTimer += Time.deltaTime;
            if (stealTimer >= stealResetTimer)
            {
                isStealing = false;
                stealTimer = 0f;
            }
        }
    }

    private void HandleInteractionInput()
    {
        if (Input.GetKeyDown(KeyCode.E))
            OnInteract();
    }

    private void HandleRaycastHighlight()
    {
        RaycastHit[] hits = Physics.RaycastAll(spawnPoint.position, spawnPoint.forward, interactionDistance);
        Debug.DrawRay(spawnPoint.position, spawnPoint.forward * interactionDistance, Color.green);

        if (hits.Length == 0)
        {
            currentCoin?.UnHighlight();
            currentCoin = null;
            canInteract = false;
            return;
        }

        RaycastHit closestHit = hits[0];
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

        if (closestHit.collider.CompareTag("Collectible"))
        {
            if (currentCoin != null && currentCoin.gameObject != closestHit.collider.gameObject)
                currentCoin.UnHighlight();

            currentCoin = closestHit.collider.GetComponent<CoinBehaviour>();
            if (currentCoin != null)
            {
                currentCoin.Highlight();
                canInteract = true;
            }
        }
    }

    private void ApplyHazardDamage()
    {
        if (isDead) return; // ✅ Prevent damage if dead
        currentHealth = Mathf.Max(currentHealth - 1, 0);
        Debug.Log("Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            isDead = true; // ✅ Set dead state
            Debug.Log("Player is dead");
            panelToggle?.ShowGameOverScreen();
        }
    }


    // Optionally make RespawnPlayer public if you want to call it from PanelToggle:
    public void Respawn()
    {
        RespawnPlayer();
    }
}

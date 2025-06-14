using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{   
    public CoinBehaviour currentCoin = null;
    public bool hasCardKey = false;
    public CardKey currentCard = null;

    public int score = 0;
    public int currentHealth = 10;
    public int maxHealth = 10;
    public int coinScore = 0;
    public bool canInteract = false;
    public bool isStealing = false;
    public float stealTimer = 0f;

    public bool isDead = false; 
    private float hazardDamageCooldown = 1f;
    private float lastHazardDamageTime = 0f;

    [SerializeField]
    private CollectibleManager collectibleManager;

    [SerializeField]
    private float interactionDistance = 1.5f;

    [SerializeField]
    private GameObject projectile;

    [SerializeField]
    private Transform spawnPoint;

    [SerializeField]
    private float fireStrength = 0f;

    [SerializeField]
    public PanelToggle panelToggle;

    [SerializeField]
    public Transform playerSpawnPoint;
    
    [Header("Hazard Settings")]
    [SerializeField] private AudioClip hazardSound;
    [SerializeField] private float hazardSoundVolume = 0.5f;
    private AudioSource hazardAudioSource;


    public void RespawnPlayer()
    {
        if (playerSpawnPoint != null)
        {
            transform.position = playerSpawnPoint.position;
            ResetPlayer();
            ResetAllNPCs();
            ResetAllCoins();
            Debug.Log("Player has respawned!");
        }
    }

    public void ResetPlayer()
    {
        currentHealth = maxHealth;
        coinScore = 0;
        score = 0;
        canInteract = false;
        isStealing = false;
        stealTimer = 0f;
        currentCoin = null;
        isDead = false;

        panelToggle?.UpdateScoreDisplay(0);

        if (collectibleManager != null)
        {
            collectibleManager.UpdateUI(); // This will call panelToggle.UpdateCollectibleUI()
        }
        GameObject[] hazards = GameObject.FindGameObjectsWithTag("HazardArea");

        foreach (GameObject hazard in hazards)
        {
            Renderer r = hazard.GetComponent<Renderer>();
            if (r != null && r.material.HasProperty("_OriginalColor"))
            {
                r.material.color = r.material.GetColor("_OriginalColor");
            }
        }
    }   
    public void ResetAllNPCs()
    {
        // Find all NPCs in the scene
        NPC[] allNPCs = FindObjectsByType<NPC>(FindObjectsSortMode.None);
        
        foreach (NPC npc in allNPCs)
        {
            npc.ResetNPC(); // Call the ResetNPC method on each NPC
        }
        
        Debug.Log($"Reset {allNPCs.Length} NPCs");
    }
    public void ResetAllCoins()
    {
        // Get ALL coins including inactive ones
        CoinBehaviour[] allCoins = FindObjectsByType<CoinBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        
        Debug.Log($"Resetting {allCoins.Length} coins");
        
        foreach (CoinBehaviour coin in allCoins)
        {
            coin.ResetCoin();
        }
        
        // Reset the collectible manager
        CollectibleManager manager = FindAnyObjectByType<CollectibleManager>();
        if (manager != null)
        {
            manager.ResetCollectibles();
        }
        else
        {
            Debug.LogWarning("CollectibleManager not found!");
        }
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
            {
                // Store original color before changing
                if (!hazardRenderer.material.HasProperty("_OriginalColor"))
                {
                    hazardRenderer.material.SetColor("_OriginalColor", hazardRenderer.material.color);
                }
                hazardRenderer.material.color = Color.red;
            }
    
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
         if (other.CompareTag("HazardArea"))
    
        // Stop hazard sound immediately
        if (hazardAudioSource != null)
        {
            Destroy(hazardAudioSource);
            hazardAudioSource = null;
        }

    }

    private void OnInteract()
    {
        if (!canInteract) return;

        // Handle card key pickup first
        if (currentCard != null)
        {
            hasCardKey = true;
            Debug.Log("Picked up the card key!");
            
            // Destroy the card and immediately clear the reference
            Destroy(currentCard.gameObject);
            currentCard = null;
            canInteract = false;
            return;
        }


        if (currentCoin != null)
        {
            if (currentCoin.isStolenItem)
            {
                isStealing = true;
                // Start the 120-minute timer (only if not already running)
                panelToggle?.StartTheftTimer(false); 
            }
            
            currentCoin.Collect(this);
            FindAnyObjectByType<CollectibleManager>()?.CollectItem();
            currentCoin.UnHighlight();
            currentCoin = null;
            canInteract = false;
        }
    }

    public void ModifyScore(int amount)
    {
    coinScore += amount;
    panelToggle?.UpdateScoreDisplay(coinScore);
    
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

        }
}

    private void HandleInteractionInput()
    {
        if (Input.GetKeyDown(KeyCode.E))
            OnInteract();
    }

    private void HandleRaycastHighlight()
    {
        // Clear previous highlights safely
        if (currentCoin != null && currentCoin.gameObject != null)
            currentCoin.UnHighlight();

        if (currentCard != null && currentCard.gameObject != null)
            currentCard.UnHighlight();

        // Reset references
        currentCoin = null;
        currentCard = null;
        canInteract = false;

        // Raycast from center of screen (camera-based aim)
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit[] hits = Physics.RaycastAll(ray, interactionDistance);
        Debug.DrawRay(ray.origin, ray.direction * interactionDistance, Color.green);

        // No hits found
        if (hits.Length == 0)
        {
            panelToggle.ShowInteractPrompt(false);
            return;
        }

        // Find closest valid hit
        RaycastHit closestHit = hits[0];
        float closestDistance = Vector3.Distance(transform.position, closestHit.point);

        foreach (RaycastHit hit in hits)
        {
            float distance = Vector3.Distance(transform.position, hit.point);
            if (distance < closestDistance)
            {
                closestHit = hit;
                closestDistance = distance;
            }
        }

        // Handle highlighting logic
        if (closestHit.collider.CompareTag("Collectible"))
        {
            currentCoin = closestHit.collider.GetComponent<CoinBehaviour>();
            if (currentCoin != null)
            {
                currentCoin.Highlight();
                canInteract = true;
                panelToggle.ShowInteractPrompt(true); // Simplified
            }
        }
        else if (closestHit.collider.CompareTag("CardKey"))
        {
            currentCard = closestHit.collider.GetComponent<CardKey>();
            if (currentCard != null)
            {
                currentCard.Highlight();
                canInteract = true;
                panelToggle.ShowInteractPrompt(true); // Simplified
            }
        }
        else if (closestHit.collider.CompareTag("Door"))
        {
            DoorBehaviour door = closestHit.collider.GetComponent<DoorBehaviour>();
            if (door != null)
            {
                canInteract = true;
                panelToggle.ShowInteractPrompt(true); // Simplified
            }
        }
        else
        {
            panelToggle.ShowInteractPrompt(false);
        }
    }
    private void ApplyHazardDamage()
    {
    if (isDead) return; // Prevent damage if dead
    currentHealth = Mathf.Max(currentHealth - 1, 0);
    panelToggle?.UpdateHealthSprite(currentHealth);
    
    if (hazardSound != null)
    {
        AudioSource.PlayClipAtPoint(hazardSound, transform.position, hazardSoundVolume);
    }

    if (currentHealth <= 0)
        {
            isDead = true; // Set dead state
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
            ResetAllNPCs(); 
            ResetAllCoins(); 
            Debug.Log("Player has respawned!");
        }
    }

    public void ResetPlayer()
    {
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
    public void ResetAllNPCs()
    {
        // Find all NPCs in the scene
        NPC[] allNPCs = FindObjectsByType<NPC>(FindObjectsSortMode.None);
        
        foreach (NPC npc in allNPCs)
        {
            npc.ResetNPC(); // Call the ResetNPC method on each NPC
        }
        
        Debug.Log($"Reset {allNPCs.Length} NPCs");
    }
    public void ResetAllCoins()
    {
        // ✅ Include inactive coins in the search
        CoinBehaviour[] allCoins = FindObjectsOfType<CoinBehaviour>(true); // true = include inactive
        
        Debug.Log($"Found {allCoins.Length} coins to reset");
        
        foreach (CoinBehaviour coin in allCoins)
        {
            coin.ResetCoin();
        }
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
        panelToggle?.UpdateHealthSprite(currentHealth);

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

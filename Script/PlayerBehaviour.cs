using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    int score = 5;
    int currentHealth = 10;
    int maxHealth = 10;

    int CoinScore = 0;
    bool canInteract = false;

    //Store the current coin and door the player is looking at

    [SerializeField]
    float interactionDistance = 5f;

    [SerializeField]
    public GameObject projectile;

    [SerializeField]
    Transform spawnPoint;

    [SerializeField]
    float fireStrength = 0f;

    //Method to modify the player's score
    //This method takes an integer amount as a parameter
    //It adds the amount to the player's score 
    //This method is public so it can be called from other scripts

    CoinBehaviour currentCoin = null;
    DoorBehaviour currentDoor = null;

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
            else if (currentHealth >= maxHealth)
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
    void OnInteract()
    {
        if (canInteract)
        {
            if (currentCoin != null)
            {
                Debug.Log("Interacting with coin");
                currentCoin.Collect(this);
                currentCoin = null;
                canInteract = false;
            }
            else if (currentDoor != null)
            {
                Debug.Log("Interacting with door");
                currentDoor.interact();
            }
        }
        // if (canInteract == false)
        // {
        //      Debug.Log("Nothing to interact with");
        // }
    }
    public void ModifyScore(int amount)
    {
        CoinScore += amount;
        Debug.Log("Score: " + CoinScore);
    }
    void OnTriggerEnter(Collider other)
    {
        // Check if the player is looking at a collectible or door
        if (other.gameObject.CompareTag("Collectible"))
        {
            // Set the canInteract flag to true
            // Get the CoinBehaviour component from the detected  object
            Debug.Log("Player is looking at " + other.gameObject.name);
            currentCoin = other.gameObject.GetComponent<CoinBehaviour>();
            canInteract = true;
            if (currentCoin != null)
            {
                currentCoin.Highlight();// Highlight the coin
            }
        }
        else if (other.CompareTag("Door"))
        {
            canInteract = true;
            currentDoor = other.GetComponent<DoorBehaviour>();
        }

    }
    // Trigger callback when the player exits the trigger collider
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Collectible"))
        {
            // Set the canInteract flag to false
            // Set the currentCoin to null
            // This prevents the player from interacting with the coin
            if (currentCoin != null)
            {
                currentCoin.UnHighlight(); // Unhighlight the coin
            }
            currentCoin = null;
            canInteract = false;
        }
    }

    void OnFire()
    {
        // Instantiate a projectile at the spawn point
        // Store the spawned projectile to the "newProjectile" variable
        GameObject newProjectile = Instantiate(projectile, spawnPoint.position, spawnPoint.rotation);

        // Create a new Vector3 variable called fireForce
        //Set it to the forward direction of the spawn point multiplied by the fireStrength variable
        // This will determine the direction and speed of the projectile
        Vector3 fireForce = spawnPoint.forward * fireStrength;

        //Get the Rigidbody component of the new projectile
        // Add a force to the projectile defined by the fireForce variable
        newProjectile.GetComponent<Rigidbody>().AddForce(fireForce);
    }

    void Update()
    {
        RaycastHit hitInfo;
        Debug.DrawRay(spawnPoint.position, spawnPoint.forward * interactionDistance, Color.green);
        if (Physics.Raycast(spawnPoint.position, spawnPoint.forward, out hitInfo, interactionDistance))
        {
            //Debug.Log ("Raycast hit: " + hitInfo.collider.gameObject.name);
            if (hitInfo.collider.gameObject.CompareTag("Collectible"))
            {
                if (currentCoin != null)
                {
                    currentCoin.UnHighlight(); // Unhighlight the previous coin
                }
                //Set the canInteract flag to true
                // Get the CoinBehaviour component from the detected object
                canInteract = true;
                currentCoin = hitInfo.collider.gameObject.GetComponent<CoinBehaviour>();
                if (currentCoin != null)
                {
                    currentCoin.Highlight(); // Highlight the coin
                }
            }
        }
        else if (currentCoin != null)
        {
            // If the raycast does not hit a collectible, unhighlight the current coin
            currentCoin.UnHighlight();
            currentCoin = null; // Reset the current coin
        }

    }

    // Add the MarkAsThief method outside of Update()
    public void MarkAsThief()
    {
        // If you have a variable isStealing, make sure it's declared in the class
        // bool isStealing = false; // Uncomment and move to class fields if needed
        Debug.Log("You have committed theft!");
    }
}

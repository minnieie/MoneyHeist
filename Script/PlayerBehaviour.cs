using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    int score = 5;
    int currentHealth = 10;
    int maxHealth = 10;

    int CoinScore = 0;
    bool canInteract = false;

    CoinBehaviour currentCoin;

    // void OnCollisionEnter(Collision collision)
    // {

    //     if (collision.gameObject.CompareTag("Collectible"))
    //     {
    //         score++;
    //         Debug.Log("Score: " + score);
    //     }

    //     if (collision.gameObject.CompareTag("HealingArea"))
    //     {
    //         if (currentHealth < maxHealth)
    //         {
    //             ++currentHealth;
    //             Debug.Log("Current Health: " + currentHealth);
    //         }
    //         else if (currentHealth >= maxHealth)
    //         {
    //             Debug.Log("Health is already full");
    //         }

    //     }
        
    //      if (collision.gameObject.CompareTag("HazardArea"))
    //     {
    //         currentHealth--;
    //         Debug.Log("Health: " + currentHealth);
    //         if (currentHealth <= 0)
    //         {
    //             Debug.Log("Player is dead");
    //         }
    //     }
    // }
    void OnInteract()
    {
        if (canInteract)
        {
            Debug.Log("Interacting with object");
            currentCoin.Collect(this);
            currentCoin = null;
            canInteract = false;

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
        if (other.gameObject.CompareTag("Collectible"))
        {
            Debug.Log("Player is looking at " + other.gameObject.name);
            currentCoin = other.gameObject.GetComponent<CoinBehaviour>();
            canInteract = true;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Collectible"))
        {
            currentCoin = null;
            canInteract = false;
        }
    }
}

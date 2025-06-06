using UnityEngine;

public class GiftBox : MonoBehaviour
{   
    [SerializeField]
    GameObject CoinPrefab;

    [SerializeField]
    int coinsToSpawn = 3;  // Default to 3 coins

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Projectile"))
        {
            Debug.Log("GiftBox hit by projectile: " + collision.gameObject.name);

            for (int i = 0; i < coinsToSpawn; i++)
            {
                Vector3 randomOffset = new Vector3(Random.Range(-1f, 1f), 0.5f, Random.Range(-1f, 1f));
                Vector3 spawnPosition = transform.position + randomOffset;

                GameObject coin = Instantiate(CoinPrefab, spawnPosition, transform.rotation);

                // Manually reset the rotation to zero using Euler angles
                coin.transform.eulerAngles = Vector3.zero;  // no rotation, upright
            }
            Destroy(gameObject);  // Destroy the gift box after spawning coins
            Destroy(collision.gameObject);  // Destroy the projectile
        }
    }
}
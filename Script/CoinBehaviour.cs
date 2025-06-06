using UnityEngine;

public class CoinBehaviour : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] private Material highlightMat;
    private Material originalMat;
    private MeshRenderer myMeshRenderer;

    [Header("Item Properties")]
    [SerializeField] private int coinValue = 5;
    [SerializeField] private bool isStolenItem = false; // Check this in Inspector for stolen items (like jewelry)

    // Called when the player collects the item
    public void Collect(PlayerBehaviour player)
    {
        if (player != null)
        {
            player.ModifyScore(coinValue);

            // If this item is marked as stolen, notify the player
            if (isStolenItem)
            {
                player.MarkAsThief(); // You'll implement this method in PlayerBehaviour
                Debug.Log("Player has stolen an item!");
            }

            Destroy(gameObject); // Remove the item after collecting
        }
    }

    private void Start()
    {
        myMeshRenderer = GetComponent<MeshRenderer>();
        if (myMeshRenderer != null)
        {
            originalMat = myMeshRenderer.material;
        }
        else
        {
            Debug.LogWarning("MeshRenderer not found on: " + gameObject.name);
        }
    }

    public void Highlight()
    {
        if (myMeshRenderer != null && highlightMat != null)
        {
            myMeshRenderer.material = highlightMat;
        }
    }

    public void UnHighlight()
    {
        if (myMeshRenderer != null && originalMat != null)
        {
            myMeshRenderer.material = originalMat;
        }
    }
}

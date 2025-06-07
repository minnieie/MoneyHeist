using UnityEngine;

public class CoinBehaviour : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] private Material highlightMat;
    private MeshRenderer[] meshRenderers;
    private Material[] originalMats;

    [Header("Item Properties")]
    [SerializeField] private int coinValue = 5;
    [SerializeField] private bool isStolenItem = false;

    private void Start()
    {
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        if (meshRenderers != null && meshRenderers.Length > 0)
        {
            originalMats = new Material[meshRenderers.Length];
            for (int i = 0; i < meshRenderers.Length; i++)
            {
                originalMats[i] = meshRenderers[i].material;
            }
        }
        else
        {
            Debug.LogWarning("MeshRenderer not found on or under: " + gameObject.name);
        }
    }

    public void Collect(PlayerBehaviour player)
    {
        if (player != null)
        {
            player.ModifyScore(coinValue);

            if (isStolenItem)
            {
                player.MarkAsThief();
                Debug.Log("Player has stolen an item!");
            }

            UnHighlight(); // Ensure unhighlight before destruction
            Destroy(gameObject);
        }
    }

    public void Highlight()
    {
        if (meshRenderers != null && highlightMat != null)
        {
            for (int i = 0; i < meshRenderers.Length; i++)
            {
                meshRenderers[i].material = highlightMat;
            }
        }
    }

    public void UnHighlight()
    {
        if (meshRenderers != null && originalMats != null)
        {
            for (int i = 0; i < meshRenderers.Length; i++)
            {
                meshRenderers[i].material = originalMats[i];
            }
        }
    }
}

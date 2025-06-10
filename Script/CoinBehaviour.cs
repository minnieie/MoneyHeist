using UnityEngine;

public class CoinBehaviour : MonoBehaviour
{   
    [Header("Audio")]
    private AudioSource sharedAudioSource;
    
    [Header("Visuals")]
    [SerializeField] private Material highlightMat;
    private MeshRenderer[] meshRenderers;
    private Material[] originalMats;

    [Header("Item Properties")]
    [SerializeField] private int coinValue = 5;
    [SerializeField] public bool isStolenItem = false;

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

        sharedAudioSource = GameObject.Find("Collectible").GetComponent<AudioSource>();
    }

    public void Collect(PlayerBehaviour player)
    {
        if (player != null)
        {
            player.ModifyScore(coinValue);

            if (isStolenItem)
            {
                Debug.Log("Player has stolen an item!");
            }
            if (sharedAudioSource != null)
            {
                sharedAudioSource.Play();
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


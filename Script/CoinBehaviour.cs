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

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private bool isCollected = false;

    [Header("NPC Reference")]
    public NPC npcReference;
    private void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;

        // Get all mesh renderers for highlighting
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        if (meshRenderers != null && meshRenderers.Length > 0)
        {
            originalMats = new Material[meshRenderers.Length];
            for (int i = 0; i < meshRenderers.Length; i++)
            {
                originalMats[i] = meshRenderers[i].material;
            }
        }

        // Get audio source
        sharedAudioSource = GameObject.Find("Collectible").GetComponent<AudioSource>();


    }



    public void Collect(PlayerBehaviour player)
    {
        if (player != null && !isCollected)
        {
            isCollected = true;

            // Update player score
            player.ModifyScore(coinValue);

            // Handle stolen item logic
            if (isStolenItem)
            {
                Debug.Log("Player has stolen an item!");
                if (!SunRotates.Instance.IsNight)
                {
                    player.isStealing = true;
                    player.stealTimer = 0f;

                    if (player.panelToggle != null && !player.panelToggle.isTheftTimerRunning)
                        player.panelToggle.StartTheftTimer();
                }
            }

            // Play sound
            if (sharedAudioSource != null)
                sharedAudioSource.Play();

            // Visual feedback
            UnHighlight();

            // Deactivate instead of destroy
            gameObject.SetActive(false);
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

    public void ResetCoin()
    {
        gameObject.SetActive(true); // Force reactivate
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        UnHighlight();
        isCollected = false;
}
    public void SetNPCReference(NPC npc)
    {
        npcReference = npc;
    }

}




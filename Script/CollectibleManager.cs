using UnityEngine;
using UnityEngine.Events;

public class CollectibleManager : MonoBehaviour
{
    [Header("UI Reference")]
    public PanelToggle panelToggle;

    [Header("Capacity Settings")]
    public int maxCollectiblesAllowed = 40;

    [Header("Settings")]
    public int totalCollectibles = 0; // Count of original collectibles only
    [SerializeField] public int collectedCount = 0;
    public int collectiblesRequiredToUnlock = 3;
    public DoorBehaviour[] doorsToUnlock;

    [Header("Events")]
    public UnityEvent onAllCollectiblesFound;
    public UnityEvent onUnlockConditionMet;

    public event System.Action<int, int> OnCollectibleCountChanged;

    private int originalCollectibleCount;

    private void Start()
    {
        totalCollectibles = CountOriginalCollectibles();
        originalCollectibleCount = totalCollectibles; 
        Debug.Log($"[Start] Initialized totalCollectibles: {totalCollectibles}");
        UpdateUI();
    }

    public void CollectItem()
    {
        if (collectedCount < totalCollectibles) // Prevent overcounting beyond 40
        {   
            collectedCount++;
            Debug.Log($"[CollectItem] Collected: {collectedCount}/{totalCollectibles}");
            UpdateUI();
            CheckDoorConditions();
        }
    }

    public void RegisterNewCollectible(GameObject collectible)
    {
        // Ensure dynamically added collectibles are tracked properly
        totalCollectibles++;
        Debug.Log($"[RegisterNewCollectible] Updated totalCollectibles: {totalCollectibles}");
        UpdateUI();
    }

    public void ResetCollectibles()
    {
        collectedCount = 0;

        // Remove NPC-dropped collectibles
        NPC[] allNPCs = FindObjectsByType<NPC>(FindObjectsSortMode.None);
        foreach (NPC npc in allNPCs)
        {
            npc.CleanDroppedItems();
        }

        // Reset only original collectibles
        CoinBehaviour[] allCoins = FindObjectsByType<CoinBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (CoinBehaviour coin in allCoins)
        {
            if (coin.npcReference == null) // Only original collectibles
                coin.ResetCoin();
        }

        // Ensure totalCollectibles is restored to original
        totalCollectibles = originalCollectibleCount;
        Debug.Log($"[ResetCollectibles] Reset totalCollectibles to: {totalCollectibles}");
        UpdateUI();
    }

    private int CountOriginalCollectibles()
    {
        GameObject[] collectibles = GameObject.FindGameObjectsWithTag("Collectible");
        int count = collectibles.Length;
        Debug.Log($"[CountOriginalCollectibles] Found collectibles by tag: {count}");
        return count;
    }

    public void UpdateUI()
    {
        panelToggle?.UpdateCollectibleUI(collectedCount, totalCollectibles);
        OnCollectibleCountChanged?.Invoke(collectedCount, totalCollectibles);
        Debug.Log($"[UpdateUI] UI Updated: {collectedCount}/{totalCollectibles}");
    }

    private void CheckDoorConditions()
    {
        if (collectedCount >= collectiblesRequiredToUnlock)
        {
            foreach (var door in doorsToUnlock) door?.UnlockDoor();
            onUnlockConditionMet?.Invoke();
        }

        if (collectedCount >= totalCollectibles)
            onAllCollectiblesFound?.Invoke();
    }
}

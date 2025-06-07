using System.Collections.Generic;
using UnityEngine;

public class CoinHighlighter : MonoBehaviour
{
    [SerializeField] private float highlightDistance = 0.5f;
    private Transform player;

    private List<CoinBehaviour> coins = new List<CoinBehaviour>();
    private HashSet<CoinBehaviour> highlightedCoins = new HashSet<CoinBehaviour>();

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        coins.AddRange(FindObjectsOfType<CoinBehaviour>());
    }

    private void Update()
    {
        if (player == null) return;

        foreach (var coin in coins)
        {
            if (coin == null) continue;

            float dist = Vector3.Distance(player.position, coin.transform.position);

            if (dist <= highlightDistance)
            {
                if (!highlightedCoins.Contains(coin))
                {
                    coin.Highlight();
                    highlightedCoins.Add(coin);
                }
            }
            else
            {
                if (highlightedCoins.Contains(coin))
                {
                    coin.UnHighlight();
                    highlightedCoins.Remove(coin);
                }
            }
        }
    }
}

using UnityEngine;

public class CoinBehaviour : MonoBehaviour
{   
    CoinBehaviour currentCoin;
    [SerializeField]
    int coinValue = 5;

    public void Collect(PlayerBehaviour player)
    {
        player.ModifyScore(coinValue);
        Destroy(gameObject);
    }
}

using UnityEngine;

public class GoldCoin : MonoBehaviour
{
    [SerializeField] private int coinValue = 100;

    public int GetValue()
    {
        return coinValue;
    }
}

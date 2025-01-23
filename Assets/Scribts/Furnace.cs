using UnityEngine;

public class Furnace : MonoBehaviour
{
    [SerializeField] private Transform coinSpawnPoint;
    [SerializeField] private GameObject goldCoinPrefab;
    [SerializeField] private ConveyorBelt conveyorBelt;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<ZombieBody>(out ZombieBody body))
        {
            SpawnGoldCoin();
            Destroy(body.gameObject);
        }
    }

    private void SpawnGoldCoin()
    {
        GameObject coin = Instantiate(goldCoinPrefab, coinSpawnPoint.position, Quaternion.identity);
        if (conveyorBelt != null)
        {
            coin.transform.position = conveyorBelt.GetSpawnPoint().position;
        }
    }
}

using UnityEngine;

public class Furnace : MonoBehaviour
{
    [SerializeField] private Transform coinSpawnPoint;
    [SerializeField] private GameObject goldCoinPrefab;
    [SerializeField] private GameObject silverCoinPrefab;
    [SerializeField] private ConveyorBelt conveyorBelt;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<ZombieBody>(out ZombieBody body))
        {
            SpawnGoldCoin(body.transform.position);
            Destroy(body.gameObject);
        }
        else if (other.TryGetComponent<ZombieHalf>(out ZombieHalf half))
        {
            SpawnSilverCoin(half.transform.position, half.GetSilverCoinValue());
            Destroy(half.gameObject);
        }
    }

    private void SpawnGoldCoin(Vector3 spawnPosition)
    {
        GameObject coin = Instantiate(goldCoinPrefab, spawnPosition, Quaternion.identity);
        SetupCoinPhysics(coin);
    }

    private void SpawnSilverCoin(Vector3 spawnPosition, int value)
    {
        GameObject coin = Instantiate(silverCoinPrefab, spawnPosition, Quaternion.identity);
        SetupCoinPhysics(coin);
    }

    private void SetupCoinPhysics(GameObject coin)
    {
        if (!coin.GetComponent<Rigidbody2D>())
        {
            Rigidbody2D rb = coin.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
        if (!coin.GetComponent<CircleCollider2D>())
        {
            coin.AddComponent<CircleCollider2D>();
        }
    }
}

using UnityEngine;

public class Furnace : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private Transform coinSpawnPoint;
    [SerializeField] private GameObject goldCoinPrefab;
    [SerializeField] private GameObject silverCoinPrefab;
    [SerializeField] private float coinSpawnOffset = 0.5f;
    [SerializeField] private float spawnHeightVariation = 0.2f;

    [Header("Force Settings")]
    [SerializeField] private float baseEjectionForce = 5f;
    [SerializeField] private float randomForceVariation = 2f;

    private int silverCoinsToSpawn = 0;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<ZombieBody>(out ZombieBody body))
        {
            SpawnGoldCoins();
            Destroy(body.gameObject);
        }
        else if (other.TryGetComponent<ZombieHalf>(out ZombieHalf half))
        {
            silverCoinsToSpawn++;
            if (silverCoinsToSpawn >= 2)
            {
                SpawnSilverCoins();
                silverCoinsToSpawn = 0;
            }
            Destroy(half.gameObject);
        }
    }

    private void SpawnGoldCoins()
    {
        // Spawn two gold coins with offset
        Vector3 leftPosition = coinSpawnPoint.position + Vector3.left * coinSpawnOffset;
        Vector3 rightPosition = coinSpawnPoint.position + Vector3.right * coinSpawnOffset;

        SpawnCoinWithForce(goldCoinPrefab, leftPosition);
        SpawnCoinWithForce(goldCoinPrefab, rightPosition);
    }

    private void SpawnSilverCoins()
    {
        // Spawn two silver coins with offset
        Vector3 leftPosition = coinSpawnPoint.position + Vector3.left * coinSpawnOffset;
        Vector3 rightPosition = coinSpawnPoint.position + Vector3.right * coinSpawnOffset;

        SpawnCoinWithForce(silverCoinPrefab, leftPosition);
        SpawnCoinWithForce(silverCoinPrefab, rightPosition);
    }

    private void SpawnCoinWithForce(GameObject coinPrefab, Vector3 position)
    {
        // Add slight height variation
        position.y += Random.Range(-spawnHeightVariation, spawnHeightVariation);

        GameObject coin = Instantiate(coinPrefab, position, Quaternion.identity);
        Rigidbody2D rb = coin.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            // Calculate random ejection force
            float randomForce = baseEjectionForce + Random.Range(-randomForceVariation, randomForceVariation);
            Vector2 ejectionDirection = new Vector2(Random.Range(-0.5f, 0.5f), 1f).normalized;
            rb.AddForce(ejectionDirection * randomForce, ForceMode2D.Impulse);
        }
    }
}

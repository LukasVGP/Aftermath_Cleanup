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
            SpawnCoins(body.transform.position);
            Destroy(body.gameObject);
        }
        else if (other.TryGetComponent<ZombieHalf>(out ZombieHalf half))
        {
            SpawnSilverCoin(half.transform.position, half.GetSilverCoinValue());
            Destroy(half.gameObject);
        }
    }

    private void SpawnCoins(Vector3 spawnPosition)
    {
        // Spawn both gold and silver coins
        GameObject goldCoin = Instantiate(goldCoinPrefab, coinSpawnPoint.position, Quaternion.identity);
        GameObject silverCoin = Instantiate(silverCoinPrefab, coinSpawnPoint.position + Vector3.right, Quaternion.identity);

        SetupCoinPhysics(goldCoin);
        SetupCoinPhysics(silverCoin);
    }

    private void SpawnSilverCoin(Vector3 spawnPosition, int value)
    {
        GameObject coin = Instantiate(silverCoinPrefab, coinSpawnPoint.position, Quaternion.identity);
        SetupCoinPhysics(coin);
    }

    private void SetupCoinPhysics(GameObject coin)
    {
        Rigidbody2D rb = coin.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = coin.AddComponent<Rigidbody2D>();
        }
        rb.bodyType = RigidbodyType2D.Dynamic;

        CircleCollider2D collider = coin.GetComponent<CircleCollider2D>();
        if (collider == null)
        {
            collider = coin.AddComponent<CircleCollider2D>();
        }
        collider.isTrigger = true;
    }
}

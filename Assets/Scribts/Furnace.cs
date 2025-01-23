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
            Vector3 zombiePosition = body.transform.position;
            Destroy(body.gameObject);
            SpawnGoldCoin(zombiePosition);
        }
    }

    private void SpawnGoldCoin(Vector3 spawnPosition)
    {
        GameObject coin = Instantiate(goldCoinPrefab, spawnPosition, Quaternion.identity);

        // Add necessary components to the coin
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

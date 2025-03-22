using UnityEngine;
using System.Collections;

public class ZombieSpawner : MonoBehaviour
{
    [Header("Movement Points")]
    [SerializeField] private Transform moveInPoint;
    [SerializeField] private Transform moveOutPoint;
    [SerializeField] private Transform zombieDropPoint;
    [SerializeField] private Transform spawnPoint2;
    [SerializeField] private Transform spawnPoint3;
    [SerializeField] private Transform spawnPoint4;
    [SerializeField] private Transform spawnPoint5;

    [Header("Spawner Settings")]
    [SerializeField] private GameObject zombiePrefab;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float dropDelay = 0.5f;
    [SerializeField] private float stopDistance = 0.01f;
    [SerializeField] private float pauseAtMoveInPoint = 0.5f;
    [SerializeField] private float spawnReturnInterval = 10f;

    private int currentWave = 0;
    private int zombiesInScene = 0;
    private int zombiePartsInScene = 0;
    private bool isMoving = false;
    private float zombieWidth;
    private Transform[] spawnPoints;
    private float nextSpawnTime;
    private bool isEnabled = true;

    private void Start()
    {
        transform.position = moveOutPoint.position;
        zombieDropPoint.position = moveOutPoint.position;
        nextSpawnTime = Time.time + spawnReturnInterval;

        // Initialize spawn points array
        spawnPoints = new Transform[] { moveInPoint, spawnPoint2, spawnPoint3, spawnPoint4, spawnPoint5 };

        if (zombiePrefab.GetComponent<Collider2D>() != null)
        {
            zombieWidth = zombiePrefab.GetComponent<Collider2D>().bounds.size.x;
        }
        else
        {
            zombieWidth = 1f;
            Debug.LogWarning("No Collider2D found on zombie prefab, using default width of 1");
        }

        StartNextWave();
    }

    private void Update()
    {
        if (!isEnabled) return;

        if (Time.time >= nextSpawnTime && !isMoving && zombiesInScene == 0 && zombiePartsInScene == 0)
        {
            nextSpawnTime = Time.time + spawnReturnInterval;
            StartNextWave();
        }
    }

    private void StartNextWave()
    {
        if (!isMoving && zombiesInScene == 0 && zombiePartsInScene == 0)
        {
            currentWave++;

            // Check if we've reached the maximum number of waves set in GameManager
            if (currentWave <= GameManager.Instance.GetRequiredWaves())
            {
                Debug.Log($"Starting Wave {currentWave}");
                StartCoroutine(SpawnWave());
            }
            else
            {
                isEnabled = false;
                GameManager.Instance.WaveCompleted();
                Debug.Log("All waves completed!");
            }
        }
    }

    private IEnumerator SpawnWave()
    {
        isMoving = true;

        // Determine how many zombies to spawn based on current wave
        // Each wave adds one more zombie
        int zombiesToSpawn = currentWave;
        Debug.Log($"Wave {currentWave}: Spawning {zombiesToSpawn} zombies");

        // For each zombie in this wave
        for (int zombieIndex = 0; zombieIndex < zombiesToSpawn; zombieIndex++)
        {
            // Use a spawn point based on the zombie index (cycling through available points)
            int spawnPointIndex = zombieIndex % spawnPoints.Length;
            Transform currentSpawnPoint = spawnPoints[spawnPointIndex];
            Vector3 targetPosition = currentSpawnPoint.position;
            zombieDropPoint.position = targetPosition;

            Debug.Log($"Moving to spawn point {spawnPointIndex + 1} for zombie {zombieIndex + 1}");

            // Move to the spawn point
            while ((targetPosition - transform.position).magnitude > stopDistance)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position, targetPosition, moveSpeed * Time.deltaTime
                );
                yield return null;
            }

            transform.position = targetPosition;
            yield return new WaitForSeconds(pauseAtMoveInPoint);

            // Spawn a zombie at this spawn point
            GameObject zombie = Instantiate(zombiePrefab, zombieDropPoint.position, zombiePrefab.transform.rotation);
            zombiesInScene++;
            StartCoroutine(MonitorZombieDestruction(zombie));

            Debug.Log($"Spawned zombie {zombieIndex + 1} at point {spawnPointIndex + 1}, total zombies: {zombiesInScene}");

            yield return new WaitForSeconds(dropDelay);
        }

        Debug.Log("Moving back to exit point");

        // Move back to the exit point
        while ((moveOutPoint.position - transform.position).magnitude > stopDistance)
        {
            transform.position = Vector3.MoveTowards(
                transform.position, moveOutPoint.position, moveSpeed * Time.deltaTime
            );
            yield return null;
        }

        transform.position = moveOutPoint.position;
        zombieDropPoint.position = moveOutPoint.position;
        isMoving = false;

        // Notify GameManager that a wave has started
        GameManager.Instance.WaveStarted(currentWave);

        Debug.Log($"Wave {currentWave} spawning complete, waiting for zombies to be destroyed");
    }

    private IEnumerator MonitorZombieDestruction(GameObject zombie)
    {
        // Wait until the zombie is destroyed
        while (zombie != null)
        {
            yield return new WaitForSeconds(0.5f);
        }

        zombiesInScene--;
        Debug.Log($"Zombie destroyed, remaining zombies: {zombiesInScene}");

        // Check if all zombies are gone
        if (zombiesInScene == 0 && zombiePartsInScene == 0)
        {
            Debug.Log("All zombies destroyed, wave completed");
            GameManager.Instance.WaveCompleted();

            // If this was the last wave, don't start a new one
            if (currentWave >= GameManager.Instance.GetRequiredWaves())
            {
                isEnabled = false;
                Debug.Log("All waves completed!");
            }
            else
            {
                // Set the next spawn time
                nextSpawnTime = Time.time + spawnReturnInterval;
            }
        }
    }

    // Called by ZombieController when a zombie part is created
    public void ZombiePartCreated()
    {
        zombiePartsInScene++;
        Debug.Log($"Zombie part created, total parts: {zombiePartsInScene}");
    }

    // Called by ZombieController when a zombie part is destroyed
    public void ZombiePartDestroyed()
    {
        zombiePartsInScene--;
        Debug.Log($"Zombie part destroyed, remaining parts: {zombiePartsInScene}");

        // Check if all zombies and parts are gone
        if (zombiesInScene == 0 && zombiePartsInScene == 0)
        {
            Debug.Log("All zombies and parts destroyed, wave completed");
            GameManager.Instance.WaveCompleted();

            // If this was the last wave, don't start a new one
            if (currentWave >= GameManager.Instance.GetRequiredWaves())
            {
                isEnabled = false;
                Debug.Log("All waves completed!");
            }
            else
            {
                // Set the next spawn time
                nextSpawnTime = Time.time + spawnReturnInterval;
            }
        }
    }

    // Public method to enable/disable the spawner
    public void SetEnabled(bool enabled)
    {
        isEnabled = enabled;
    }

    // Public method to get the current wave
    public int GetCurrentWave()
    {
        return currentWave;
    }
}

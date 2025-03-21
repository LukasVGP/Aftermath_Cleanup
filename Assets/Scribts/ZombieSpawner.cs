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

        spawnPoints = new Transform[] {
            moveInPoint,
            spawnPoint2,
            spawnPoint3,
            spawnPoint4,
            spawnPoint5
        };

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
            if (currentWave <= GameManager.Instance.GetRequiredWaves())
            {
                Debug.Log($"Starting Wave {currentWave}");
                StartCoroutine(SpawnWave());
            }
            else
            {
                isEnabled = false;
                GameManager.Instance.WaveCompleted();
            }
        }
    }

    private IEnumerator SpawnWave()
    {
        isMoving = true;

        // Determine how many spawn points to use based on current wave
        int spawnPointsToUse = Mathf.Min(currentWave, spawnPoints.Length);

        Debug.Log($"Wave {currentWave}: Using {spawnPointsToUse} spawn points");

        // For each active spawn point in this wave
        for (int spawnPointIndex = 0; spawnPointIndex < spawnPointsToUse; spawnPointIndex++)
        {
            Transform currentSpawnPoint = spawnPoints[spawnPointIndex];
            Vector3 targetPosition = currentSpawnPoint.position;
            zombieDropPoint.position = targetPosition;

            Debug.Log($"Moving to spawn point {spawnPointIndex + 1}");

            // Move to the spawn point
            while ((targetPosition - transform.position).magnitude > stopDistance)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetPosition,
                    moveSpeed * Time.deltaTime
                );
                yield return null;
            }

            transform.position = targetPosition;
            yield return new WaitForSeconds(pauseAtMoveInPoint);

            // Spawn a single zombie at this spawn point
            GameObject zombie = Instantiate(zombiePrefab, zombieDropPoint.position, zombiePrefab.transform.rotation);
            zombiesInScene++;
            StartCoroutine(MonitorZombieDestruction(zombie));

            Debug.Log($"Spawned zombie at point {spawnPointIndex + 1}, total zombies: {zombiesInScene}");

            yield return new WaitForSeconds(dropDelay);
        }

        Debug.Log("Moving back to exit point");

        // Move back to the exit point
        while ((moveOutPoint.position - transform.position).magnitude > stopDistance)
        {
            transform.position = Vector3.MoveTowards(transform.position, moveOutPoint.position, moveSpeed * Time.deltaTime);
            zombieDropPoint.position = transform.position;
            yield return null;
        }

        transform.position = moveOutPoint.position;
        zombieDropPoint.position = moveOutPoint.position;
        isMoving = false;

        GameManager.Instance.WaveCompleted();

        if (zombiesInScene == 0 && zombiePartsInScene == 0)
        {
            yield return new WaitForSeconds(spawnReturnInterval);
            StartNextWave();
        }
    }

    private IEnumerator MonitorZombieDestruction(GameObject zombie)
    {
        while (zombie != null)
        {
            yield return null;
        }
        OnZombieDestroyed();
    }

    public void OnZombieDestroyed()
    {
        zombiesInScene--;
        GameManager.Instance.AddDisposedZombie();
        Debug.Log($"Zombie destroyed. Remaining zombies: {zombiesInScene}");
    }

    public void OnZombieTornApart()
    {
        zombiesInScene--;
        zombiePartsInScene += 2;
        Debug.Log($"Zombie torn apart. Parts in scene: {zombiePartsInScene}");
    }

    public void OnZombiePartDestroyed()
    {
        zombiePartsInScene--;
        Debug.Log($"Zombie part destroyed. Remaining parts: {zombiePartsInScene}");
    }
}

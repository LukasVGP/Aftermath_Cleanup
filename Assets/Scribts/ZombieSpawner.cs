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
    [SerializeField] private float zombieSpacing = 1.2f;
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

        for (int spawnPointIndex = 0; spawnPointIndex < spawnPoints.Length; spawnPointIndex++)
        {
            Transform currentSpawnPoint = spawnPoints[spawnPointIndex];
            Vector3 targetPosition = currentSpawnPoint.position;
            zombieDropPoint.position = targetPosition;

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

            Vector3 moveDirection = (moveOutPoint.position - targetPosition).normalized;
            float spacingDistance = zombieWidth * zombieSpacing;
            Vector3 currentSpawnPosition = targetPosition;

            int zombiesToSpawn = Mathf.CeilToInt(currentWave / spawnPoints.Length);
            if (spawnPointIndex < currentWave % spawnPoints.Length) zombiesToSpawn++;

            for (int i = 0; i < zombiesToSpawn; i++)
            {
                zombieDropPoint.position = currentSpawnPosition;
                GameObject zombie = Instantiate(zombiePrefab, zombieDropPoint.position, zombiePrefab.transform.rotation);
                zombiesInScene++;
                StartCoroutine(MonitorZombieDestruction(zombie));
                yield return new WaitForSeconds(dropDelay);

                if (i < zombiesToSpawn - 1)
                {
                    Vector3 nextPosition = new Vector3(
                        currentSpawnPosition.x + (moveDirection.x * spacingDistance),
                        currentSpawnPoint.position.y,
                        currentSpawnPosition.z
                    );
                    currentSpawnPosition = nextPosition;

                    while ((nextPosition - transform.position).magnitude > stopDistance)
                    {
                        transform.position = Vector3.MoveTowards(transform.position, nextPosition, moveSpeed * Time.deltaTime);
                        zombieDropPoint.position = transform.position;
                        yield return null;
                    }
                }
            }
        }

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

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

    [Header("Wave Settings")]
    [Range(1, 5)]
    [SerializeField] private int spawnRounds = 3;
    [Range(1, 5)]
    [SerializeField] private int zombiesPerSpawn = 3;
    [Range(1, 5)]
    [SerializeField] private int activeSpawnPoints = 3;
    [SerializeField] private float timeBetweenWaves = 2f;
    [SerializeField] private float pauseAtMoveInPoint = 0.5f;

    private int currentRound = 0;
    private int totalZombies;
    private int zombiesInScene = 0;
    private int zombiePartsInScene = 0;
    private bool isMoving = false;
    private float zombieWidth;
    private Transform[] spawnPoints;

    private void Start()
    {
        totalZombies = spawnRounds * zombiesPerSpawn;
        GameManager.Instance?.SetRequiredZombies(totalZombies);

        transform.position = moveOutPoint.position;
        zombieDropPoint.position = moveOutPoint.position;

        spawnPoints = new Transform[activeSpawnPoints];
        spawnPoints[0] = moveInPoint;

        if (activeSpawnPoints > 1) spawnPoints[1] = spawnPoint2;
        if (activeSpawnPoints > 2) spawnPoints[2] = spawnPoint3;
        if (activeSpawnPoints > 3) spawnPoints[3] = spawnPoint4;
        if (activeSpawnPoints > 4) spawnPoints[4] = spawnPoint5;

        if (zombiePrefab.GetComponent<Collider2D>() != null)
        {
            zombieWidth = zombiePrefab.GetComponent<Collider2D>().bounds.size.x;
        }
        else
        {
            zombieWidth = 1f;
            Debug.LogWarning("No Collider2D found on zombie prefab, using default width of 1");
        }

        StartNextRound();
    }

    private void StartNextRound()
    {
        if (!isMoving && zombiesInScene == 0 && zombiePartsInScene == 0)
        {
            currentRound++;
            if (currentRound <= spawnRounds)
            {
                Debug.Log($"Starting Round {currentRound}");
                StartCoroutine(SpawnWave());
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

            int zombiesToSpawn = zombiesPerSpawn / activeSpawnPoints;
            if (spawnPointIndex < zombiesPerSpawn % activeSpawnPoints) zombiesToSpawn++;

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

        if (zombiesInScene == 0 && zombiePartsInScene == 0)
        {
            yield return new WaitForSeconds(timeBetweenWaves);
            StartNextRound();
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
        GameManager.Instance?.AddDisposedZombie();

        if (zombiesInScene == 0 && zombiePartsInScene == 0 && !isMoving)
        {
            StartCoroutine(DelayedNextRound());
        }
    }

    public void OnZombieTornApart()
    {
        zombiesInScene--;
        zombiePartsInScene += 2;
    }

    public void OnZombiePartDestroyed()
    {
        zombiePartsInScene--;
        GameManager.Instance?.AddDisposedZombie(0.5f);

        if (zombiesInScene == 0 && zombiePartsInScene == 0 && !isMoving)
        {
            StartCoroutine(DelayedNextRound());
        }
    }

    private IEnumerator DelayedNextRound()
    {
        yield return new WaitForSeconds(timeBetweenWaves);
        StartNextRound();
    }
}

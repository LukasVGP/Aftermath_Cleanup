using UnityEngine;
using System.Collections;

public class ZombieSpawner : MonoBehaviour
{
    [Header("Movement Points")]
    [SerializeField] private Transform moveInPoint;
    [SerializeField] private Transform moveOutPoint;
    [SerializeField] private Transform zombieDropPoint;

    [Header("Spawner Settings")]
    [SerializeField] private GameObject zombiePrefab;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float dropDelay = 0.5f;
    [SerializeField] private float stopDistance = 0.01f;

    [Header("Wave Settings")]
    [SerializeField] private int maxWaves = 5;
    [SerializeField] private float timeBetweenWaves = 2f;
    [SerializeField] private float pauseAtMoveInPoint = 0.5f;

    private int currentWave = 0;
    private int zombiesInScene = 0;
    private int zombiePartsInScene = 0;
    private bool isMoving = false;

    private void Start()
    {
        transform.position = moveOutPoint.position;
        StartNextWave();
    }

    private void StartNextWave()
    {
        if (!isMoving && zombiesInScene == 0 && zombiePartsInScene == 0)
        {
            currentWave++;
            if (currentWave <= maxWaves)
            {
                Debug.Log($"Starting Wave {currentWave}");
                StartCoroutine(SpawnWave());
            }
            else
            {
                GameManager.Instance?.TriggerGameEnd();
            }
        }
    }

    private IEnumerator SpawnWave()
    {
        isMoving = true;

        while ((moveInPoint.position - transform.position).magnitude > stopDistance)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                moveInPoint.position,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }

        transform.position = moveInPoint.position;
        yield return new WaitForSeconds(pauseAtMoveInPoint);

        for (int i = 0; i < currentWave; i++)
        {
            GameObject zombie = Instantiate(
                zombiePrefab,
                zombieDropPoint.position,
                zombiePrefab.transform.rotation
            );

            zombiesInScene++;
            Debug.Log($"Spawned zombie {i + 1} of {currentWave}");
            StartCoroutine(MonitorZombieDestruction(zombie));
            yield return new WaitForSeconds(dropDelay);
        }

        while ((moveOutPoint.position - transform.position).magnitude > stopDistance)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                moveOutPoint.position,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }

        transform.position = moveOutPoint.position;
        isMoving = false;
        Debug.Log($"Wave {currentWave} complete");

        if (zombiesInScene == 0 && zombiePartsInScene == 0)
        {
            yield return new WaitForSeconds(timeBetweenWaves);
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
        Debug.Log($"Zombie destroyed. Remaining zombies: {zombiesInScene}");
        if (zombiesInScene == 0 && zombiePartsInScene == 0 && !isMoving)
        {
            StartCoroutine(DelayedNextWave());
        }
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
        if (zombiesInScene == 0 && zombiePartsInScene == 0 && !isMoving)
        {
            StartCoroutine(DelayedNextWave());
        }
    }

    private IEnumerator DelayedNextWave()
    {
        yield return new WaitForSeconds(timeBetweenWaves);
        StartNextWave();
    }
}

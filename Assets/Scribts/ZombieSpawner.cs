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

    [Header("Wave Settings")]
    [SerializeField] private int maxWaves = 5;
    [SerializeField] private float timeBetweenWaves = 2f;

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
        currentWave++;
        if (currentWave <= maxWaves)
        {
            StartCoroutine(SpawnWave());
        }
        else
        {
            GameManager.Instance.TriggerGameEnd();
        }
    }

    private IEnumerator SpawnWave()
    {
        Debug.Log($"Wave {currentWave} starting");
        isMoving = true;

        // Stage 1: Move In
        while (Vector3.Distance(transform.position, moveInPoint.position) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, moveInPoint.position, moveSpeed * Time.deltaTime);
            yield return null;
        }

        // Stage 2: Drop Zombies
        transform.position = moveInPoint.position;
        for (int i = 0; i < currentWave; i++)
        {
            GameObject zombie = Instantiate(zombiePrefab, zombieDropPoint.position, zombiePrefab.transform.rotation);
            zombiesInScene++;
            Debug.Log($"Zombie {i + 1} spawned");
            yield return new WaitForSeconds(dropDelay);
        }

        // Stage 3: Move Out
        while (Vector3.Distance(transform.position, moveOutPoint.position) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, moveOutPoint.position, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = moveOutPoint.position;
        isMoving = false;
        Debug.Log($"Wave {currentWave} complete");
    }

    private IEnumerator MonitorZombieDestruction(GameObject zombie)
    {
        while (zombie != null)
        {
            yield return null;
        }

        zombiesInScene--;
        if (zombiesInScene == 0 && zombiePartsInScene == 0 && !isMoving)
        {
            yield return new WaitForSeconds(timeBetweenWaves);
            StartNextWave();
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

    private void OnDrawGizmos()
    {
        if (moveInPoint != null && moveOutPoint != null && zombieDropPoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(moveInPoint.position, 0.5f);
            Gizmos.DrawWireSphere(moveOutPoint.position, 0.5f);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(zombieDropPoint.position, 0.5f);

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(moveOutPoint.position, moveInPoint.position);
        }
    }
}

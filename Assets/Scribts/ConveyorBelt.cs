using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint1;
    [SerializeField] private Transform spawnPoint2;
    [SerializeField] private Transform endPoint;
    [SerializeField] private float beltSpeed = 2f;
    [SerializeField] private Vector2 moveDirection = Vector2.right;
    private bool isActive = false;
    private Animator animator;
    private int currentSpawnPoint = 1;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public Transform GetSpawnPoint()
    {
        if (currentSpawnPoint == 1)
        {
            currentSpawnPoint = 2;
            return spawnPoint1;
        }
        else
        {
            currentSpawnPoint = 1;
            return spawnPoint2;
        }
    }

    private void Update()
    {
        if (isActive)
        {
            MoveItems();
        }
    }

    private void MoveItems()
    {
        Vector2 searchStart = new Vector2(
            Mathf.Min(spawnPoint1.position.x, spawnPoint2.position.x),
            Mathf.Min(spawnPoint1.position.y, spawnPoint2.position.y)
        );

        Collider2D[] items = Physics2D.OverlapAreaAll(searchStart, new Vector2(endPoint.position.x, endPoint.position.y));

        foreach (Collider2D item in items)
        {
            bool isZombieBody = item.TryGetComponent(out ZombieBody zombieBody);
            bool isZombieHalf = item.TryGetComponent(out ZombieHalf zombieHalf);
            bool isGoldCoin = item.TryGetComponent(out GoldCoin goldCoin);

            if ((isZombieBody && !zombieBody.IsBeingCarried()) ||
                (isZombieHalf && !zombieHalf.IsBeingCarried()) ||
                isGoldCoin)
            {
                Vector3 newPosition = item.transform.position + (Vector3)(moveDirection.normalized * beltSpeed * Time.deltaTime);
                item.transform.position = newPosition;

                if (Vector2.Distance(item.transform.position, endPoint.position) < 0.1f)
                {
                    if (isZombieBody || isZombieHalf)
                    {
                        Destroy(item.gameObject);
                    }
                    if (isGoldCoin)
                    {
                        EnableCoinPhysics(goldCoin);
                    }
                }
            }
        }
    }

    private void EnableCoinPhysics(GoldCoin coin)
    {
        Rigidbody2D rb = coin.gameObject.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 1;
        }
    }

    public void Activate()
    {
        isActive = true;
        if (animator) animator.SetBool("IsMoving", true);
    }

    public void Deactivate()
    {
        isActive = false;
        if (animator) animator.SetBool("IsMoving", false);
    }
}

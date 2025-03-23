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
    private FurnaceLid furnaceLid;

    void Start()
    {
        animator = GetComponent<Animator>();
        furnaceLid = FindAnyObjectByType<FurnaceLid>();
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

            // Check if the item can be moved
            bool canMove = false;
            if (isZombieBody && !zombieBody.IsBeingCarried())
            {
                canMove = true;
            }
            else if (isZombieHalf && !zombieHalf.IsBeingCarried())
            {
                canMove = true;
            }

            // If the item can be moved and we're approaching the furnace, check if lid is open
            if (canMove)
            {
                // Check if we're near the furnace and the lid is closed
                bool nearFurnace = Vector2.Distance(item.transform.position, endPoint.position) < 1.0f;
                bool lidClosed = furnaceLid != null && !furnaceLid.CanZombiesPassThrough();
                if (nearFurnace && lidClosed && (isZombieBody || isZombieHalf))
                {
                    // Don't move zombies if the furnace lid is closed
                    continue;
                }

                // Move the item
                Vector3 newPosition = item.transform.position + (Vector3)(moveDirection.normalized * beltSpeed * Time.deltaTime);
                item.transform.position = newPosition;

                // Check if the item has reached the end point
                if (Vector2.Distance(item.transform.position, endPoint.position) < 0.1f)
                {
                    if (isZombieBody || isZombieHalf)
                    {
                        // Only destroy if the furnace lid is open
                        if (furnaceLid == null || furnaceLid.CanZombiesPassThrough())
                        {
                            Destroy(item.gameObject);
                        }
                    }
                }
            }
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

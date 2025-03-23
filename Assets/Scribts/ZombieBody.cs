using UnityEngine;

public class ZombieBody : MonoBehaviour
{
    [Header("Grab Points")]
    public Transform headPosition;
    public Transform feetPosition;
    [SerializeField] private float grabRadius = 0.5f;
    [SerializeField] private float carryHeight = 1.5f;

    [Header("Tearing Settings")]
    [SerializeField] private float baseBodyLength = 2f;
    [SerializeField] private float tearMultiplier = 2f;
    [SerializeField] private GameObject upperHalfPrefab;
    [SerializeField] private GameObject lowerHalfPrefab;
    [SerializeField] private GameObject intestinesPrefab;
    [SerializeField] private Transform tearPoint;

    private MonoBehaviour headCarrier;
    private MonoBehaviour feetCarrier;
    private bool isBeingCarried = false;
    private bool hasTornApart = false;
    private Rigidbody2D rb;
    private Quaternion defaultRotation;
    private ConveyorBelt conveyorBelt;
    private bool isOnFurnaceConveyor = false;
    private FurnaceLid furnaceLid;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        defaultRotation = transform.rotation;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.mass = 100f;
        rb.linearDamping = 10f;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        gameObject.layer = 8;

        // Find the furnace lid in the scene
        furnaceLid = FindAnyObjectByType<FurnaceLid>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("ConveyorBelt") && !isBeingCarried)
        {
            if (collision.gameObject.TryGetComponent(out ConveyorBelt belt))
            {
                conveyorBelt = belt;
                Vector3 spawnPosition = conveyorBelt.GetSpawnPoint().position;
                transform.SetPositionAndRotation(spawnPosition, defaultRotation);
                rb.linearVelocity = Vector2.zero;
                rb.bodyType = RigidbodyType2D.Kinematic;
                isOnFurnaceConveyor = true;
            }
        }

        // Check if this is the furnace lid and it's closed
        if (collision.gameObject.GetComponent<FurnaceLid>() != null)
        {
            FurnaceLid lid = collision.gameObject.GetComponent<FurnaceLid>();
            if (!lid.CanZombiesPassThrough())
            {
                // Bounce back or stop
                rb.linearVelocity = new Vector2(-rb.linearVelocity.x * 0.5f, rb.linearVelocity.y);
                Debug.Log("Zombie body hit closed furnace lid");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if we've entered the furnace
        if (other.CompareTag("Furnace") && isOnFurnaceConveyor)
        {
            // Only allow entry if the lid is open
            if (furnaceLid != null && furnaceLid.CanZombiesPassThrough())
            {
                Debug.Log("Zombie body entered furnace");
                GameManager.Instance?.AddDisposedZombie();
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.TryGetComponent(out RedPlayerController redPlayer) && redPlayer.WantsToGrab)
        {
            CheckGrabPoints(redPlayer);
        }
        if (other.TryGetComponent(out BluePlayerController bluePlayer) && bluePlayer.WantsToGrab)
        {
            CheckGrabPoints(bluePlayer);
        }
    }

    private void CheckGrabPoints(MonoBehaviour player)
    {
        float distanceToHead = Vector2.Distance(player.transform.position, headPosition.position);
        float distanceToFeet = Vector2.Distance(player.transform.position, feetPosition.position);

        if (distanceToHead <= grabRadius && headCarrier == null)
        {
            headCarrier = player;
            if (player is RedPlayerController redPlayer) redPlayer.GrabBody(this);
            if (player is BluePlayerController bluePlayer) bluePlayer.GrabBody(this);
        }

        if (distanceToFeet <= grabRadius && feetCarrier == null)
        {
            feetCarrier = player;
            if (player is RedPlayerController redPlayer) redPlayer.GrabBody(this);
            if (player is BluePlayerController bluePlayer) bluePlayer.GrabBody(this);
        }

        UpdateCarryState();
    }

    private void UpdateCarryState()
    {
        bool wasCarried = isBeingCarried;
        isBeingCarried = (headCarrier != null && feetCarrier != null);

        if (isBeingCarried)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            isOnFurnaceConveyor = false;
        }
        else if (wasCarried)
        {
            DropBody();
        }
    }

    private void Update()
    {
        if (isBeingCarried && headCarrier != null && feetCarrier != null)
        {
            float currentDistance = Vector2.Distance(headCarrier.transform.position, feetCarrier.transform.position);

            if (currentDistance >= baseBodyLength * tearMultiplier)
            {
                TearApart();
            }
            else
            {
                Vector3 midPoint = (headCarrier.transform.position + feetCarrier.transform.position) / 2f;
                midPoint.y += carryHeight;
                transform.SetPositionAndRotation(midPoint, defaultRotation);
            }
        }
    }

    private void TearApart()
    {
        if (hasTornApart || !enabled) return;

        hasTornApart = true;

        // Spawn upper half with carrier
        if (upperHalfPrefab != null && headPosition != null)
        {
            Quaternion upperRotation = Quaternion.Euler(0, 0, -90f);
            GameObject upperHalf = Instantiate(upperHalfPrefab, headPosition.position + Vector3.up * 2f, upperRotation);
            var upperScript = upperHalf.GetComponent<ZombieHalf>();

            if (upperScript != null && headCarrier != null)
            {
                // Instead of calling SetCarrier, manually set up the carrier relationship
                if (headCarrier is RedPlayerController redPlayer)
                {
                    redPlayer.SetCarryingZombieHalf(true);
                    redPlayer.GrabBody(null);
                }
                else if (headCarrier is BluePlayerController bluePlayer)
                {
                    bluePlayer.SetCarryingZombieHalf(true);
                    bluePlayer.GrabBody(null);
                }

                // Trigger the OnTriggerStay2D in the ZombieHalf to pick up the object
                // This is a workaround since we can't directly call SetCarrier
                upperHalf.transform.position = headCarrier.transform.position + Vector3.up * 2f;
            }
        }

        // Spawn lower half with carrier
        if (lowerHalfPrefab != null && feetPosition != null)
        {
            Quaternion lowerRotation = Quaternion.Euler(0, 0, -90f);
            GameObject lowerHalf = Instantiate(lowerHalfPrefab, feetPosition.position + Vector3.up * 2f, lowerRotation);
            var lowerScript = lowerHalf.GetComponent<ZombieHalf>();

            if (lowerScript != null && feetCarrier != null)
            {
                // Instead of calling SetCarrier, manually set up the carrier relationship
                if (feetCarrier is RedPlayerController redPlayer)
                {
                    redPlayer.SetCarryingZombieHalf(true);
                    redPlayer.GrabBody(null);
                }
                else if (feetCarrier is BluePlayerController bluePlayer)
                {
                    bluePlayer.SetCarryingZombieHalf(true);
                    bluePlayer.GrabBody(null);
                }

                // Trigger the OnTriggerStay2D in the ZombieHalf to pick up the object
                // This is a workaround since we can't directly call SetCarrier
                lowerHalf.transform.position = feetCarrier.transform.position + Vector3.up * 2f;
            }
        }

        // Spawn intestines
        if (intestinesPrefab != null && tearPoint != null)
        {
            GameObject intestine = Instantiate(intestinesPrefab, tearPoint.position, Quaternion.identity);
            var intestineRb = intestine.GetComponent<Rigidbody2D>();
            if (intestineRb != null)
            {
                intestineRb.bodyType = RigidbodyType2D.Dynamic;
                intestineRb.gravityScale = 5f;
                intestineRb.mass = 1f;
                intestineRb.linearDamping = 0.1f;
                intestineRb.constraints = RigidbodyConstraints2D.None;
                intestineRb.AddForce(new Vector2(Random.Range(-2f, 2f), -10f), ForceMode2D.Impulse);
            }
        }

        Destroy(gameObject);
    }

    private void DropBody()
    {
        headCarrier = null;
        feetCarrier = null;
        isBeingCarried = false;
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
        }
    }

    public void Release(MonoBehaviour carrier)
    {
        if (carrier == headCarrier) headCarrier = null;
        if (carrier == feetCarrier) feetCarrier = null;
        UpdateCarryState();
    }

    public bool IsBeingCarried() => isBeingCarried;
}

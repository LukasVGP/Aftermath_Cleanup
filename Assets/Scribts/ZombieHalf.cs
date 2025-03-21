using UnityEngine;

public class ZombieHalf : MonoBehaviour
{
    [Header("Half Settings")]
    [SerializeField] private bool isUpperHalf;
    [SerializeField] private bool isLowerHalf;
    [SerializeField] private int silverCoinValue = 25;
    [SerializeField] private float carryHeight = 2f;

    private Rigidbody2D rb;
    private MonoBehaviour carrier;
    private bool isBeingCarried = false;
    private ConveyorBelt conveyorBelt;
    private Quaternion defaultRotation;
    private bool isOnConveyor = false;
    private Collider2D myCollider;
    private FurnaceLid furnaceLid;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<Collider2D>();
        defaultRotation = Quaternion.Euler(0, 0, -90f);

        // Initialize rigidbody properties
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.mass = 50f;
        rb.linearDamping = 10f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.gravityScale = 1f;
        gameObject.layer = 8;

        GameObject[] zombieHalves = GameObject.FindGameObjectsWithTag("ZombieHalf");
        foreach (GameObject other in zombieHalves)
        {
            if (other != gameObject && other.GetComponent<Collider2D>() != null)
            {
                Physics2D.IgnoreCollision(myCollider, other.GetComponent<Collider2D>(), true);
            }
        }

        var redPlayer = GameObject.FindGameObjectWithTag("RedPlayer");
        var bluePlayer = GameObject.FindGameObjectWithTag("BluePlayer");

        if (redPlayer != null && redPlayer.GetComponent<Collider2D>() != null)
        {
            Physics2D.IgnoreCollision(myCollider, redPlayer.GetComponent<Collider2D>(), true);
        }

        if (bluePlayer != null && bluePlayer.GetComponent<Collider2D>() != null)
        {
            Physics2D.IgnoreCollision(myCollider, bluePlayer.GetComponent<Collider2D>(), true);
        }

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
                isOnConveyor = true;
                rb.constraints = RigidbodyConstraints2D.FreezeAll;
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
                Debug.Log("Zombie half hit closed furnace lid");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if we've entered the furnace
        if (other.CompareTag("Furnace") && isOnConveyor)
        {
            // Only allow entry if the lid is open
            if (furnaceLid != null && furnaceLid.CanZombiesPassThrough())
            {
                Debug.Log("Zombie half entered furnace");

                // Award silver coins to the appropriate player
                if (carrier is RedPlayerController)
                {
                    GameManager.Instance?.Player1CollectCoin(silverCoinValue);
                }
                else if (carrier is BluePlayerController)
                {
                    GameManager.Instance?.Player2CollectCoin(silverCoinValue);
                }

                GameManager.Instance?.AddDisposedZombie(0.5f); // Half a zombie counts as 0.5
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // Check for player interactions
        if (!isBeingCarried)
        {
            // For Red Player
            if (other.TryGetComponent(out RedPlayerController redPlayer))
            {
                if (redPlayer.WantsToGrab && !redPlayer.IsCarryingAnything())
                {
                    PickUp(redPlayer);
                }
            }

            // For Blue Player
            if (other.TryGetComponent(out BluePlayerController bluePlayer))
            {
                if (bluePlayer.WantsToGrab && !bluePlayer.IsCarryingAnything())
                {
                    PickUp(bluePlayer);
                }
            }
        }
    }

    private void PickUp(MonoBehaviour player)
    {
        carrier = player;
        isBeingCarried = true;
        isOnConveyor = false;

        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        // Update player state
        if (player is RedPlayerController redPlayer)
        {
            redPlayer.SetCarryingZombieHalf(true);
            redPlayer.GrabBody(null);
        }
        else if (player is BluePlayerController bluePlayer)
        {
            bluePlayer.SetCarryingZombieHalf(true);
            bluePlayer.GrabBody(null);
        }

        Debug.Log($"Zombie half picked up by {player.name}");
    }

    private void Update()
    {
        if (isBeingCarried && carrier != null)
        {
            // Position the zombie half above the carrier
            Vector3 offset = Vector3.up * carryHeight;
            transform.position = carrier.transform.position + offset;
            transform.rotation = defaultRotation;

            // Check if the player wants to release
            bool shouldRelease = false;

            if (carrier is RedPlayerController redPlayer)
            {
                shouldRelease = !redPlayer.WantsToGrab;
            }
            else if (carrier is BluePlayerController bluePlayer)
            {
                shouldRelease = !bluePlayer.WantsToGrab;
            }

            if (shouldRelease)
            {
                Release();
            }
        }
    }

    public void Release()
    {
        if (carrier is RedPlayerController redPlayer)
        {
            redPlayer.SetCarryingZombieHalf(false);
        }
        else if (carrier is BluePlayerController bluePlayer)
        {
            bluePlayer.SetCarryingZombieHalf(false);
        }

        Debug.Log($"Zombie half released by {(carrier != null ? carrier.name : "unknown")}");

        carrier = null;
        isBeingCarried = false;

        if (rb != null)
        {
            if (!isOnConveyor)
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
                rb.gravityScale = 3f;
                rb.mass = 2f;
                rb.linearDamping = 0.2f;
                rb.simulated = true;
                rb.WakeUp();

                Vector3 releasePosition = transform.position;
                releasePosition.y += 1f;
                transform.position = releasePosition;
            }

            transform.rotation = defaultRotation;
        }
    }

    // This is the method that ConveyorBelt will call
    public bool IsBeingCarried()
    {
        return isBeingCarried;
    }

    public int GetSilverCoinValue() => silverCoinValue;
    public bool IsUpperHalf() => isUpperHalf;
    public bool IsLowerHalf() => isLowerHalf;
}

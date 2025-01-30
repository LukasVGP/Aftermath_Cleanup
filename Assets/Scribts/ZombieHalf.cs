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

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<Collider2D>();
        defaultRotation = Quaternion.Euler(0, 0, -90f);
        InitializeRigidbody();

        GameObject[] zombieHalves = GameObject.FindGameObjectsWithTag("ZombieHalf");
        foreach (GameObject other in zombieHalves)
        {
            if (other != gameObject)
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
    }

    private void InitializeRigidbody()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.mass = 50f;
        rb.linearDamping = 10f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.gravityScale = 1f;
        gameObject.layer = 8;
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
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // Only proceed if this zombie half isn't being carried AND isn't on conveyor
        if (!isBeingCarried && !isOnConveyor)
        {
            RedPlayerController redPlayerController;
            BluePlayerController bluePlayerController;

            // For Red Player
            if (other.TryGetComponent(out redPlayerController))
            {
                // Triple check to ensure player can pick up
                if (redPlayerController.WantsToGrab &&
                    !redPlayerController.IsCarryingAnything() &&
                    !redPlayerController.IsCarryingZombieHalf() &&
                    carrier == null)
                {
                    SetCarrier(redPlayerController);
                    redPlayerController.GrabBody(null);
                }
            }

            // For Blue Player
            if (other.TryGetComponent(out bluePlayerController))
            {
                // Triple check to ensure player can pick up
                if (bluePlayerController.WantsToGrab &&
                    !bluePlayerController.IsCarryingAnything() &&
                    !bluePlayerController.IsCarryingZombieHalf() &&
                    carrier == null)
                {
                    SetCarrier(bluePlayerController);
                    bluePlayerController.GrabBody(null);
                }
            }
        }
    }

    public void SetCarrier(MonoBehaviour newCarrier)
    {
        carrier = newCarrier;
        isBeingCarried = (carrier != null);

        if (isBeingCarried && rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.linearVelocity = Vector2.zero;
            Vector3 offset = Vector3.up * carryHeight;
            transform.position = carrier.transform.position + offset;
            transform.rotation = defaultRotation;
            isOnConveyor = false;

            if (carrier is RedPlayerController redPlayerController)
            {
                redPlayerController.SetCarryingZombieHalf(true);
            }
            else if (carrier is BluePlayerController bluePlayerController)
            {
                bluePlayerController.SetCarryingZombieHalf(true);
            }
        }
    }

    private void Update()
    {
        if (isBeingCarried && carrier != null)
        {
            Vector3 offset = Vector3.up * carryHeight;
            transform.position = carrier.transform.position + offset;
            transform.rotation = defaultRotation;

            if ((carrier is RedPlayerController red && !red.WantsToGrab) ||
                (carrier is BluePlayerController blue && !blue.WantsToGrab))
            {
                Release();
            }
        }
    }

    public void Release()
    {
        if (carrier is RedPlayerController redPlayerController)
        {
            redPlayerController.SetCarryingZombieHalf(false);
        }
        else if (carrier is BluePlayerController bluePlayerController)
        {
            bluePlayerController.SetCarryingZombieHalf(false);
        }

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

    public bool IsBeingCarried() => isBeingCarried;
    public int GetSilverCoinValue() => silverCoinValue;
    public bool IsUpperHalf() => isUpperHalf;
    public bool IsLowerHalf() => isLowerHalf;
}

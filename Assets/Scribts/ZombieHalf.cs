using UnityEngine;

public class ZombieHalf : MonoBehaviour
{
    [Header("Half Settings")]
    [SerializeField] private bool isUpperHalf;
    [SerializeField] private bool isLowerHalf;
    [SerializeField] private int silverCoinValue = 25;
    [SerializeField] private float grabRadius = 0.5f;
    [SerializeField] private float carryHeight = 2f;

    private Rigidbody2D rb;
    private MonoBehaviour carrier;
    private bool isBeingCarried = false;
    private bool hasBeenPickedUp = false;
    private ConveyorBelt conveyorBelt;
    private Quaternion defaultRotation;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        defaultRotation = transform.rotation;
        InitializeRigidbody();
    }

    private void InitializeRigidbody()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
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
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!isBeingCarried && !hasBeenPickedUp)
        {
            if (other.TryGetComponent(out RedPlayerController redPlayer) && redPlayer.WantsToGrab)
            {
                SetCarrier(redPlayer);
                redPlayer.GrabBody(null);
                hasBeenPickedUp = true;
            }
            if (other.TryGetComponent(out BluePlayerController bluePlayer) && bluePlayer.WantsToGrab)
            {
                SetCarrier(bluePlayer);
                bluePlayer.GrabBody(null);
                hasBeenPickedUp = true;
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
        }
    }

    private void Update()
    {
        if (isBeingCarried && carrier != null)
        {
            Vector3 offset = Vector3.up * carryHeight;
            transform.position = carrier.transform.position + offset;

            if ((carrier is RedPlayerController red && !red.WantsToGrab) ||
                (carrier is BluePlayerController blue && !blue.WantsToGrab))
            {
                Release();
            }
        }
    }

    // Update the Release method
    public void Release()
    {
        carrier = null;
        isBeingCarried = false;
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Removed FreezePositionX
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 3f; // Increased gravity for faster falling
            rb.mass = 2f; // Increased mass
            rb.linearDamping = 0.2f; // Reduced drag
        }
    }


    public bool IsBeingCarried() => isBeingCarried;
    public int GetSilverCoinValue() => silverCoinValue;
    public bool IsUpperHalf() => isUpperHalf;
    public bool IsLowerHalf() => isLowerHalf;
}

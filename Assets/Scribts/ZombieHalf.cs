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

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        defaultRotation = Quaternion.Euler(0, 0, -90f);
        InitializeRigidbody();
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
        if (!isBeingCarried)
        {
            if (other.TryGetComponent(out RedPlayerController redPlayer) && redPlayer.WantsToGrab)
            {
                SetCarrier(redPlayer);
                redPlayer.GrabBody(null);
            }
            if (other.TryGetComponent(out BluePlayerController bluePlayer) && bluePlayer.WantsToGrab)
            {
                SetCarrier(bluePlayer);
                bluePlayer.GrabBody(null);
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
            }
            transform.rotation = defaultRotation;
        }
    }

    public bool IsBeingCarried() => isBeingCarried;
    public int GetSilverCoinValue() => silverCoinValue;
    public bool IsUpperHalf() => isUpperHalf;
    public bool IsLowerHalf() => isLowerHalf;
}

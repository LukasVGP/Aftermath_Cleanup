using UnityEngine;

public class ZombieBody : MonoBehaviour
{
    [Header("Grab Points")]
    public Transform headPosition;
    public Transform feetPosition;
    [SerializeField] private float grabRadius = 0.5f;
    [SerializeField] private float carryHeight = 1.5f;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float spawnHeightOffset = -0.5f;

    private MonoBehaviour headCarrier;
    private MonoBehaviour feetCarrier;
    private bool isBeingCarried = false;
    private Rigidbody2D rb;
    private Quaternion defaultRotation;
    private ConveyorBelt conveyorBelt;

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
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("ConveyorBelt") && !isBeingCarried)
        {
            if (collision.gameObject.TryGetComponent(out ConveyorBelt belt))
            {
                conveyorBelt = belt;
                Vector3 spawnPosition = conveyorBelt.GetSpawnPoint().position;
                spawnPosition.y += spawnHeightOffset;
                transform.SetPositionAndRotation(spawnPosition, defaultRotation);
                rb.linearVelocity = Vector2.zero;
                rb.bodyType = RigidbodyType2D.Kinematic;
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
        }
        else if (wasCarried)
        {
            DropBody();
        }
    }

    public void Release(MonoBehaviour player)
    {
        if (player == headCarrier) headCarrier = null;
        if (player == feetCarrier) feetCarrier = null;
        UpdateCarryState();
    }

    private void DropBody()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
        headCarrier = null;
        feetCarrier = null;
        isBeingCarried = false;
        transform.rotation = defaultRotation;
    }

    public void OnDropped()
    {
        DropBody();
    }

    private void Update()
    {
        if (isBeingCarried && headCarrier != null && feetCarrier != null)
        {
            Vector3 midPoint = (headCarrier.transform.position + feetCarrier.transform.position) / 2f;
            midPoint.y += carryHeight;
            transform.SetPositionAndRotation(midPoint, defaultRotation);
        }
    }

    public bool IsBeingCarried() => isBeingCarried;
}

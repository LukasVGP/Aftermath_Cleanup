using UnityEngine;

public class ZombieBody : MonoBehaviour
{
    [Header("Grab Points")]
    public Transform headPosition;
    public Transform feetPosition;
    [SerializeField] private float grabRadius = 0.5f;

    private MonoBehaviour headCarrier;
    private MonoBehaviour feetCarrier;
    private bool isBeingCarried = false;
    private Rigidbody2D rb;
    private Quaternion defaultRotation;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        defaultRotation = transform.rotation;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        var redPlayer = other.GetComponent<RedPlayerController>();
        var bluePlayer = other.GetComponent<BluePlayerController>();

        if (redPlayer != null && redPlayer.WantsToGrab)
        {
            CheckGrabPoints(redPlayer);
        }
        if (bluePlayer != null && bluePlayer.WantsToGrab)
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
        else if (distanceToFeet <= grabRadius && feetCarrier == null)
        {
            feetCarrier = player;
            if (player is RedPlayerController redPlayer) redPlayer.GrabBody(this);
            if (player is BluePlayerController bluePlayer) bluePlayer.GrabBody(this);
        }

        isBeingCarried = (headCarrier != null && feetCarrier != null);
        if (isBeingCarried)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.linearVelocity = Vector2.zero;
        }
    }

    public void Release(MonoBehaviour player)
    {
        if (player == headCarrier) headCarrier = null;
        if (player == feetCarrier) feetCarrier = null;

        if (headCarrier == null || feetCarrier == null)
        {
            DropBody();
        }
    }

    private void DropBody()
    {
        isBeingCarried = false;
        rb.bodyType = RigidbodyType2D.Dynamic;
        headCarrier = null;
        feetCarrier = null;
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
            transform.position = midPoint;
            transform.rotation = defaultRotation;
        }
    }

    public bool IsBeingCarried() => isBeingCarried;
}

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
    [SerializeField] private StrainMeter strainMeter;

    private MonoBehaviour headCarrier;
    private MonoBehaviour feetCarrier;
    private bool isBeingCarried = false;
    private bool hasTornApart = false;
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

            if (strainMeter != null)
            {
                strainMeter.gameObject.SetActive(true);
                strainMeter.SetTarget(transform);
            }
        }
        else if (wasCarried)
        {
            if (strainMeter != null)
            {
                strainMeter.gameObject.SetActive(false);
                strainMeter.SetTarget(null);
            }
            DropBody();
        }
    }

    private void Update()
    {
        if (isBeingCarried && headCarrier != null && feetCarrier != null)
        {
            float currentDistance = Vector2.Distance(headCarrier.transform.position, feetCarrier.transform.position);

            if (strainMeter != null)
            {
                strainMeter.UpdateStrain(currentDistance);
            }

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

        if (upperHalfPrefab != null && headPosition != null)
        {
            GameObject upperHalf = Instantiate(upperHalfPrefab, headPosition.position + Vector3.up * 2f, Quaternion.identity);
            var upperScript = upperHalf.GetComponent<ZombieHalf>();
            if (upperScript != null && headCarrier != null)
            {
                upperScript.SetCarrier(headCarrier);
            }
        }

        if (lowerHalfPrefab != null && feetPosition != null)
        {
            GameObject lowerHalf = Instantiate(lowerHalfPrefab, feetPosition.position + Vector3.up * 2f, Quaternion.identity);
            var lowerScript = lowerHalf.GetComponent<ZombieHalf>();
            if (lowerScript != null && feetCarrier != null)
            {
                lowerScript.SetCarrier(feetCarrier);
            }
        }

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
                intestineRb.constraints = RigidbodyConstraints2D.FreezeRotation;
                intestineRb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
                intestineRb.AddForce(new Vector2(Random.Range(-2f, 2f), -10f), ForceMode2D.Impulse);
            }
        }

        Destroy(gameObject);
    }

    public void Release(MonoBehaviour releaser)
    {
        if (releaser == headCarrier)
        {
            headCarrier = null;
        }
        else if (releaser == feetCarrier)
        {
            feetCarrier = null;
        }

        UpdateCarryState();
    }

    private void DropBody()
    {
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb.gravityScale = 1f;
        }
    }

    public bool IsBeingCarried()
    {
        return isBeingCarried;
    }
}

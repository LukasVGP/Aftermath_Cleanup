using UnityEngine;

public class ZombieHalf : MonoBehaviour
{
    [Header("Half Settings")]
    [SerializeField] private bool isUpperHalf;
    [SerializeField] private bool isLowerHalf;
    [SerializeField] private int silverCoinValue = 25;
    [SerializeField] private float grabRadius = 0.5f;

    private Rigidbody2D rb;
    private MonoBehaviour carrier;
    private bool isBeingCarried = false;
    private bool hasBeenPickedUp = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        InitializeRigidbody();
    }

    private void InitializeRigidbody()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.mass = 1f;
        rb.linearDamping = 0.5f;
    }

    public void InitializeHalf(bool isUpper, bool isLower)
    {
        isUpperHalf = isUpper;
        isLowerHalf = isLower;
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
            Vector3 offset = isUpperHalf ? Vector3.up : (isLowerHalf ? Vector3.down : Vector3.zero);
            transform.position = carrier.transform.position + offset;
        }
    }

    private void Update()
    {
        if (isBeingCarried && carrier != null)
        {
            Vector3 offset = isUpperHalf ? Vector3.up : (isLowerHalf ? Vector3.down : Vector3.zero);
            transform.position = carrier.transform.position + offset;

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
            rb.bodyType = RigidbodyType2D.Dynamic;
        }
    }

    public int GetSilverCoinValue() => silverCoinValue;
    public bool IsUpperHalf() => isUpperHalf;
    public bool IsLowerHalf() => isLowerHalf;
}

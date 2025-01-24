using UnityEngine;

public class ZombieHalf : MonoBehaviour
{
    [SerializeField] private bool isUpperHalf;
    [SerializeField] private int silverCoinValue = 50;

    private Rigidbody2D rb;
    private MonoBehaviour carrier;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
    }

    private void Start()
    {
        InitializeRigidbody();
    }

    private void InitializeRigidbody()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.mass = 1f;
        rb.linearDamping = 0.5f;
    }

    public void SetCarrier(MonoBehaviour newCarrier)
    {
        if (newCarrier != null)
        {
            carrier = newCarrier;
            if (rb != null)
            {
                rb.bodyType = RigidbodyType2D.Kinematic;
            }
        }
    }

    public void Release()
    {
        carrier = null;
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
        }
    }

    public int GetSilverCoinValue() => silverCoinValue;
}

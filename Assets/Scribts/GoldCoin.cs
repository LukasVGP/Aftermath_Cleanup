using UnityEngine;

public class GoldCoin : MonoBehaviour
{
    [SerializeField] private int coinValue = 100;
    private bool canBePickedUp = false;
    private Rigidbody2D rb;
    private CircleCollider2D collider;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        collider = GetComponent<CircleCollider2D>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<CircleCollider2D>();
        }
        collider.isTrigger = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            canBePickedUp = true;
        }

        if (canBePickedUp && (collision.gameObject.CompareTag("RedPlayer") || collision.gameObject.CompareTag("BluePlayer")))
        {
            GameManager.Instance?.AddScore(coinValue);
            Destroy(gameObject);
        }
    }

    public int GetValue()
    {
        return coinValue;
    }
}

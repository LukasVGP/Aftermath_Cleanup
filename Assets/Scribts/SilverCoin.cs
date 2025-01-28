using UnityEngine;

public class SilverCoin : MonoBehaviour
{
    [SerializeField] private int coinValue = 25; // 1/4 of gold coin value
    private bool canBePickedUp = false;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            canBePickedUp = true;
        }

        if (canBePickedUp && (collision.gameObject.CompareTag("RedPlayer") ||
            collision.gameObject.CompareTag("BluePlayer")))
        {
            GameManager.Instance?.AddScore(coinValue);
            Destroy(gameObject);
        }
    }
}

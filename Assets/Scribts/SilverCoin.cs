using UnityEngine;

public class SilverCoin : MonoBehaviour
{
    [SerializeField] private int coinValue = 25;
    private GameManager gameManager;
    private Rigidbody2D rb;
    private CircleCollider2D triggerCollider;
    private CircleCollider2D physicsCollider;

    void Start()
    {
        gameManager = Object.FindFirstObjectByType<GameManager>();
        rb = GetComponent<Rigidbody2D>();

        // Setup trigger collider for pickup detection
        triggerCollider = gameObject.AddComponent<CircleCollider2D>();
        triggerCollider.isTrigger = true;

        // Setup physics collider for ground collision
        physicsCollider = gameObject.AddComponent<CircleCollider2D>();
        physicsCollider.isTrigger = false;

        // Setup rigidbody
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 1f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.mass = 1f;
        rb.linearDamping = 0f;

        gameObject.tag = "Coin";
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("RedPlayer") || other.CompareTag("BluePlayer"))
        {
            Debug.Log($"Player collected silver coin worth {coinValue}!");
            if (gameManager != null)
            {
                gameManager.AddScore(coinValue);
                Destroy(gameObject);
            }
        }
    }

    public int GetValue()
    {
        return coinValue;
    }
}

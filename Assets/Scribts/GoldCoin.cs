using UnityEngine;

public class GoldCoin : MonoBehaviour
{
    [SerializeField] private int coinValue = 100;
    private GameManager gameManager;
    private Rigidbody2D rb;
    private CircleCollider2D coinCollider;

    void Start()
    {
        gameManager = Object.FindFirstObjectByType<GameManager>();
        rb = GetComponent<Rigidbody2D>();
        coinCollider = GetComponent<CircleCollider2D>();

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 1f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.mass = 1f;
        rb.linearDamping = 0f;

        coinCollider.isTrigger = false;
        gameObject.tag = "Coin";
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("RedPlayer") || collision.gameObject.CompareTag("BluePlayer"))
        {
            Debug.Log($"Player collected gold coin worth {coinValue}!");
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

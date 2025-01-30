using UnityEngine;

public class BluePlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isCarryingZombieHalf = false;
    public bool WantsToGrab { get; private set; }
    private ZombieBody carriedBody;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Debug.Log("Blue Player initialized");
    }

    private void Update()
    {
        HandleGrabbing();
        HandleJump();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        float horizontalMovement = 0f;
        if (Input.GetKey(KeyCode.Keypad4)) horizontalMovement -= 1f;
        if (Input.GetKey(KeyCode.Keypad6)) horizontalMovement += 1f;
        rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocity.y);
    }

    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Keypad8) && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            Debug.Log("Blue Player jumped");
        }
    }

    private void HandleGrabbing()
    {
        if (Input.GetKeyDown(KeyCode.Keypad7))
        {
            WantsToGrab = true;
            Debug.Log("Blue Player wants to grab");
        }
        else if (Input.GetKeyUp(KeyCode.Keypad7))
        {
            WantsToGrab = false;
            if (carriedBody != null)
            {
                carriedBody.Release(this);
                carriedBody = null;
            }
            Debug.Log("Blue Player released grab");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Coin"))
        {
            Debug.Log($"Blue Player collided with coin: {other.gameObject.name}");
            int scoreValue = 0;

            if (other.TryGetComponent<GoldCoin>(out GoldCoin goldCoin))
            {
                scoreValue = goldCoin.GetValue();
            }
            else if (other.TryGetComponent<SilverCoin>(out SilverCoin silverCoin))
            {
                scoreValue = silverCoin.GetValue();
            }

            if (scoreValue > 0)
            {
                GameManager.Instance?.AddScore(scoreValue);
                Destroy(other.gameObject);
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    public void GrabBody(ZombieBody body)
    {
        carriedBody = body;
        Debug.Log("Blue Player grabbed zombie body");
    }

    public bool IsCarryingZombieHalf() => isCarryingZombieHalf;
    public bool IsCarryingAnything() => isCarryingZombieHalf || carriedBody != null;

    public void SetCarryingZombieHalf(bool carrying)
    {
        isCarryingZombieHalf = carrying;
        Debug.Log($"Blue Player carrying zombie half: {carrying}");
    }
}

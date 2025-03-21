using UnityEngine;

public class RedPlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isCarryingZombieHalf = false;
    public bool WantsToGrab { get; private set; }
    private ZombieBody carriedBody;

    // Static input tracking for both players
    public static bool leftKeyPressed = false;
    public static bool rightKeyPressed = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (groundCheck == null)
        {
            GameObject check = new GameObject("GroundCheck");
            check.transform.parent = transform;
            check.transform.localPosition = new Vector3(0, -0.5f, 0);
            groundCheck = check.transform;
        }
        Debug.Log("Red Player initialized");
    }

    private void Update()
    {
        CheckGrounded();
        HandleGrabbing();
        HandleJump();

        // Track WASD inputs for red player
        if (Input.GetKeyDown(KeyCode.A)) leftKeyPressed = true;
        if (Input.GetKeyUp(KeyCode.A)) leftKeyPressed = false;
        if (Input.GetKeyDown(KeyCode.D)) rightKeyPressed = true;
        if (Input.GetKeyUp(KeyCode.D)) rightKeyPressed = false;
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void HandleMovement()
    {
        float horizontalMovement = 0f;
        if (leftKeyPressed) horizontalMovement -= 1f;
        if (rightKeyPressed) horizontalMovement += 1f;

        // Use MovePosition for more reliable movement
        Vector2 targetPosition = rb.position + new Vector2(horizontalMovement * moveSpeed * Time.fixedDeltaTime, 0);
        rb.MovePosition(targetPosition);

        // Keep vertical velocity for jumping
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }

    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            Debug.Log("Red Player jumped");
        }
    }

    private void HandleGrabbing()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            WantsToGrab = true;
            Debug.Log("Red Player wants to grab");
        }
        else if (Input.GetKeyUp(KeyCode.Q))
        {
            WantsToGrab = false;
            if (carriedBody != null)
            {
                carriedBody.Release(this);
                carriedBody = null;
            }
            Debug.Log("Red Player released grab");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Coin"))
        {
            Debug.Log($"Red Player collided with coin: {other.gameObject.name}");
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

    public void GrabBody(ZombieBody body)
    {
        carriedBody = body;
        Debug.Log("Red Player grabbed zombie body");
    }

    public bool IsCarryingZombieHalf() => isCarryingZombieHalf;

    public bool IsCarryingAnything() => isCarryingZombieHalf || carriedBody != null;

    public void SetCarryingZombieHalf(bool carrying)
    {
        isCarryingZombieHalf = carrying;
        Debug.Log($"Red Player carrying zombie half: {carrying}");
    }
}

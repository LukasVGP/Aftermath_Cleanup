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
        if (Input.GetKey(KeyCode.A)) horizontalMovement -= 1f;
        if (Input.GetKey(KeyCode.D)) horizontalMovement += 1f;
        rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocity.y);
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

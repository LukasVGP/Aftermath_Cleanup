using UnityEngine;

public class BluePlayerController : MonoBehaviour
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
        Debug.Log("Blue Player initialized");
    }

    private void Update()
    {
        CheckGrounded();
        HandleGrabbing();
        HandleJump();
        // Track keypad inputs for blue player
        if (Input.GetKeyDown(KeyCode.Keypad4)) leftKeyPressed = true;
        if (Input.GetKeyUp(KeyCode.Keypad4)) leftKeyPressed = false;
        if (Input.GetKeyDown(KeyCode.Keypad6)) rightKeyPressed = true;
        if (Input.GetKeyUp(KeyCode.Keypad6)) rightKeyPressed = false;
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
        if (Input.GetKeyDown(KeyCode.RightShift) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
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

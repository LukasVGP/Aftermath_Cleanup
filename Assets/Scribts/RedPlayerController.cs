using UnityEngine;

public class RedPlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    private Rigidbody2D rb;
    private bool isGrounded;
    public bool WantsToGrab { get; private set; }
    private ZombieBody carriedBody;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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
        if (Input.GetKey(KeyCode.A)) horizontalMovement -= 1f;
        if (Input.GetKey(KeyCode.D)) horizontalMovement += 1f;
        rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocity.y);
    }

    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.W) && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    private void HandleGrabbing()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            WantsToGrab = true;
        }
        else if (Input.GetKeyUp(KeyCode.Q))
        {
            WantsToGrab = false;
            if (carriedBody != null)
            {
                carriedBody.Release(this);
                carriedBody = null;
            }
        }
    }

    public void GrabBody(ZombieBody body)
    {
        carriedBody = body;
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
}

using UnityEngine;

public class RedPlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float interactRange = 2f;

    private Rigidbody2D rb;
    private Animator animator;
    private bool isCarrying = false;
    private ZombieBody carriedBody;
    private BluePlayerController otherPlayer;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        otherPlayer = Object.FindFirstObjectByType<BluePlayerController>();
    }

    void Update()
    {
        HandleActions();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        float horizontalMovement = 0f;

        if (Input.GetKey(KeyCode.A)) horizontalMovement -= 1f;
        if (Input.GetKey(KeyCode.D)) horizontalMovement += 1f;
        if (Input.GetKey(KeyCode.W) && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocity.y);
    }

    private void HandleActions()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryInteractWithLever();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            TryLiftZombieBody();
        }
    }

    private void TryInteractWithLever()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, interactRange);
        foreach (Collider2D collider in colliders)
        {
            RedLever lever = collider.GetComponent<RedLever>();
            if (lever != null)
            {
                lever.ActivateLever();
                break;
            }
        }
    }

    private void TryLiftZombieBody()
    {
        if (isCarrying)
        {
            DropBody();
            return;
        }

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, interactRange);
        foreach (Collider2D collider in colliders)
        {
            ZombieBody body = collider.GetComponent<ZombieBody>();
            if (body != null && !body.IsBeingCarried() &&
                Vector3.Distance(transform.position, body.feetPosition.position) < interactRange)
            {
                if (otherPlayer.IsCarrying())
                {
                    isCarrying = true;
                    carriedBody = body;
                    body.OnPickedUp(this);
                }
                break;
            }
        }
    }

    private void DropBody()
    {
        if (carriedBody != null)
        {
            carriedBody.OnDropped();
            isCarrying = false;
            carriedBody = null;
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

    public bool IsCarrying() => isCarrying;
    public Vector3 GetPosition() => transform.position;
}

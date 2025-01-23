using UnityEngine;

public class BluePlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float interactRange = 2f;

    private Rigidbody2D rb;
    private Animator animator;
    private bool isCarrying = false;
    private ZombieBody carriedBody;
    private RedPlayerController otherPlayer;
    private SpriteRenderer spriteRenderer;
    private bool isFacingRight = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        otherPlayer = FindObjectOfType<RedPlayerController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        HandleMovement();
        HandleActions();
        UpdateAnimations();
    }

    private void HandleMovement()
    {
        float horizontal = 0f;
        float vertical = 0f;

        if (Input.GetKey(KeyCode.Keypad4)) horizontal -= 1f;
        if (Input.GetKey(KeyCode.Keypad6)) horizontal += 1f;
        if (Input.GetKey(KeyCode.Keypad8)) vertical += 1f;
        if (Input.GetKey(KeyCode.Keypad5)) vertical -= 1f;

        Vector2 movement = new Vector2(horizontal, vertical).normalized * moveSpeed;
        rb.linearVelocity = movement;

        // Flip sprite based on movement direction
        if (horizontal != 0)
        {
            if (horizontal > 0 && !isFacingRight)
                FlipSprite();
            else if (horizontal < 0 && isFacingRight)
                FlipSprite();
        }
    }

    private void HandleActions()
    {
        if (Input.GetKeyDown(KeyCode.Keypad9)) // Use action
        {
            TryInteractWithConveyor();
        }

        if (Input.GetKeyDown(KeyCode.Keypad7)) // Lift action
        {
            TryLiftZombieBody();
        }
    }

    private void TryInteractWithConveyor()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, interactRange);
        foreach (Collider2D collider in colliders)
        {
            ConveyorBelt belt = collider.GetComponent<ConveyorBelt>();
            if (belt != null)
            {
                belt.ActivateBlueLever();
                animator.SetTrigger("UseAction");
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
            if (body != null && !body.IsBeingCarried())
            {
                PickUpBody(body);
                break;
            }
        }
    }

    private void PickUpBody(ZombieBody body)
    {
        isCarrying = true;
        carriedBody = body;
        body.OnPickedUp(this);
        animator.SetBool("IsCarrying", true);
    }

    private void DropBody()
    {
        if (carriedBody != null)
        {
            carriedBody.OnDropped();
            isCarrying = false;
            carriedBody = null;
            animator.SetBool("IsCarrying", false);
        }
    }

    private void FlipSprite()
    {
        isFacingRight = !isFacingRight;
        spriteRenderer.flipX = !isFacingRight;
    }

    private void UpdateAnimations()
    {
        animator.SetFloat("Speed", rb.linearVelocity.magnitude);
        animator.SetBool("IsCarrying", isCarrying);
    }

    public bool IsCarrying() => isCarrying;
    public Vector3 GetPosition() => transform.position;

    private void OnDrawGizmosSelected()
    {
        // Visualize interaction range in editor
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }

    public ZombieBody GetCarriedBody() => carriedBody;

    public void ForceDropBody()
    {
        DropBody();
    }
}

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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        otherPlayer = Object.FindFirstObjectByType<RedPlayerController>();
    }

    void Update()
    {
        // Movement
        float horizontal = 0f;
        float vertical = 0f;

        if (Input.GetKey(KeyCode.Keypad4)) horizontal -= 1f;
        if (Input.GetKey(KeyCode.Keypad6)) horizontal += 1f;
        if (Input.GetKey(KeyCode.Keypad8)) vertical += 1f;
        if (Input.GetKey(KeyCode.Keypad5)) vertical -= 1f;

        Vector2 movement = new Vector2(horizontal, vertical).normalized * moveSpeed;
        rb.linearVelocity = movement;

        // Actions
        if (Input.GetKeyDown(KeyCode.Keypad9))
        {
            TryInteractWithLever();
        }

        if (Input.GetKeyDown(KeyCode.Keypad7))
        {
            TryLiftZombieBody();
        }
    }

    private void TryInteractWithLever()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, interactRange);
        foreach (Collider2D collider in colliders)
        {
            BlueLever lever = collider.GetComponent<BlueLever>();
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

    public bool IsCarrying() => isCarrying;
    public Vector3 GetPosition() => transform.position;
}

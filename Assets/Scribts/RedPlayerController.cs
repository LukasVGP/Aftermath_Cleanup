using UnityEngine;

public class RedPlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float interactRange = 2f;

    private Rigidbody2D rb;
    private Animator animator;
    private bool isCarrying = false;
    private ZombieBody carriedBody;
    private BluePlayerController otherPlayer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        otherPlayer = FindObjectOfType<BluePlayerController>();
    }

    void Update()
    {
        // Movement
        float horizontal = 0f;
        float vertical = 0f;

        if (Input.GetKey(KeyCode.A)) horizontal -= 1f;
        if (Input.GetKey(KeyCode.D)) horizontal += 1f;
        if (Input.GetKey(KeyCode.W)) vertical += 1f;
        if (Input.GetKey(KeyCode.S)) vertical -= 1f;

        Vector2 movement = new Vector2(horizontal, vertical).normalized * moveSpeed;
        rb.linearVelocity = movement;

        // Actions
        if (Input.GetKeyDown(KeyCode.E)) // Use action
        {
            TryInteractWithLever();
        }

        if (Input.GetKeyDown(KeyCode.Q)) // Lift action
        {
            TryLiftZombieBody();
        }
    }

    private void TryInteractWithLever()
    {
        // Check for nearby lever and activate it
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, interactRange);
        foreach (Collider2D collider in colliders)
        {
            FurnaceLid lid = collider.GetComponent<FurnaceLid>();
            if (lid != null)
            {
                lid.ActivateRedLever();
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

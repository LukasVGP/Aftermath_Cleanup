using UnityEngine;

public class BlueLever : MonoBehaviour
{
    [SerializeField] private float leverRotation = 90f;
    [SerializeField] private float interactionRadius = 2f;

    private Animator animator;
    private ConveyorBelt conveyorBelt;
    private BluePlayerController bluePlayer;
    private bool isActivated = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        conveyorBelt = FindFirstObjectByType<ConveyorBelt>();
        bluePlayer = FindFirstObjectByType<BluePlayerController>();

        Debug.Log("BlueLever initialized, BluePlayer found: " + (bluePlayer != null));
        Debug.Log("ConveyorBelt found: " + (conveyorBelt != null));
    }

    private void Update()
    {
        if (bluePlayer == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, bluePlayer.transform.position);

        // Only allow interaction when player is within range
        if (distanceToPlayer <= interactionRadius)
        {
            Debug.Log("Blue player in range, distance: " + distanceToPlayer);

            if (Input.GetKeyDown(KeyCode.Keypad9))
            {
                Debug.Log("Keypad9 pressed near blue lever");
                ActivateLever();
            }

            if (Input.GetKeyUp(KeyCode.Keypad9))
            {
                Debug.Log("Keypad9 released near blue lever");
                DeactivateLever();
            }
        }
        else if (isActivated && Input.GetKeyUp(KeyCode.Keypad9))
        {
            // If player moves out of range while holding the lever, deactivate it
            DeactivateLever();
        }
    }

    public void ActivateLever()
    {
        isActivated = true;
        transform.rotation = Quaternion.Euler(0, 0, leverRotation);

        if (animator) animator.SetBool("IsActivated", true);
        if (conveyorBelt) conveyorBelt.Activate();

        Debug.Log("Blue lever activated");
    }

    public void DeactivateLever()
    {
        isActivated = false;
        transform.rotation = Quaternion.Euler(0, 0, 0);

        if (animator) animator.SetBool("IsActivated", false);
        if (conveyorBelt) conveyorBelt.Deactivate();

        Debug.Log("Blue lever deactivated");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}

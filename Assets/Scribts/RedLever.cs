using UnityEngine;

public class RedLever : MonoBehaviour
{
    [SerializeField] private FurnaceLid furnaceLid;
    [SerializeField] private float interactionRadius = 2f;
    [SerializeField] private float leverRotation = 90f; // Added rotation amount

    private bool isActivated = false;
    private RedPlayerController redPlayer;
    private Animator animator; // Added animator reference

    void Start()
    {
        redPlayer = FindFirstObjectByType<RedPlayerController>();
        animator = GetComponent<Animator>(); // Get the animator component

        Debug.Log("RedLever initialized, RedPlayer found: " + (redPlayer != null));

        // Make sure we have a reference to the furnace lid
        if (furnaceLid == null)
        {
            furnaceLid = FindFirstObjectByType<FurnaceLid>();
            Debug.Log("Found furnace lid reference: " + (furnaceLid != null));
        }
    }

    void Update()
    {
        if (redPlayer == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, redPlayer.transform.position);

        // Only allow interaction when player is within range
        if (distanceToPlayer <= interactionRadius)
        {
            Debug.Log("Red player in range, distance: " + distanceToPlayer);

            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("E key pressed near red lever");
                ActivateLever();
            }

            if (Input.GetKeyUp(KeyCode.E))
            {
                Debug.Log("E key released near red lever");
                DeactivateLever();
            }
        }
        else if (isActivated && Input.GetKeyUp(KeyCode.E))
        {
            // If player moves out of range while holding the lever, deactivate it
            DeactivateLever();
        }
    }

    public void ActivateLever()
    {
        isActivated = true;
        // Rotate the lever
        transform.rotation = Quaternion.Euler(0, 0, leverRotation);

        // Set animator if available
        if (animator) animator.SetBool("IsActivated", true);

        Debug.Log("Red lever activated, furnaceLid reference exists: " + (furnaceLid != null));

        if (furnaceLid != null)
        {
            furnaceLid.ActivateRedLever();
        }
    }

    public void DeactivateLever()
    {
        isActivated = false;
        // Reset rotation
        transform.rotation = Quaternion.Euler(0, 0, 0);

        // Reset animator if available
        if (animator) animator.SetBool("IsActivated", false);

        Debug.Log("Red lever deactivated");

        if (furnaceLid != null)
        {
            furnaceLid.DeactivateLid();
        }
    }

    public bool IsActivated()
    {
        return isActivated;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}

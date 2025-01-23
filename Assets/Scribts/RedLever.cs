using UnityEngine;

public class RedLever : MonoBehaviour
{
    [SerializeField] private FurnaceLid furnaceLid;
    [SerializeField] private float interactionRadius = 2f;
    private bool isActivated = false;
    private RedPlayerController redPlayer;

    void Start()
    {
        redPlayer = FindFirstObjectByType<RedPlayerController>();
        Debug.Log("RedLever initialized, RedPlayer found: " + (redPlayer != null));
    }

    void Update()
    {
        if (redPlayer == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, redPlayer.transform.position);

        if (distanceToPlayer <= interactionRadius)
        {
            Debug.Log("Player in range, distance: " + distanceToPlayer);
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("E key pressed");
                ActivateLever();
            }
            if (Input.GetKeyUp(KeyCode.E))
            {
                Debug.Log("E key released");
                DeactivateLever();
            }
        }
    }

    public void ActivateLever()
    {
        isActivated = true;
        Debug.Log("Lever activated, furnaceLid reference exists: " + (furnaceLid != null));
        if (furnaceLid != null)
        {
            furnaceLid.ActivateRedLever();
        }
    }

    public void DeactivateLever()
    {
        isActivated = false;
        Debug.Log("Lever deactivated");
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

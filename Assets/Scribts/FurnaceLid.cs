using UnityEngine;

public class FurnaceLid : MonoBehaviour
{
    [SerializeField] private float lidRotationSpeed = 90f;
    [SerializeField] private Transform pivotPoint;
    [SerializeField] private float maxRotation = 90f;
    [SerializeField] private float openThreshold = 75f; // Threshold to consider lid fully open
    [SerializeField] private Collider2D lidCollider; // Reference to the lid's collider

    private float currentRotation = 0f;
    private bool isOpen = false;
    private bool isFullyOpen = false;

    void Start()
    {
        // If collider not assigned, try to get it from this gameObject
        if (lidCollider == null)
        {
            lidCollider = GetComponent<Collider2D>();
        }
    }

    void Update()
    {
        if (isOpen && currentRotation < maxRotation)
        {
            float rotationThisFrame = lidRotationSpeed * Time.deltaTime;
            currentRotation = Mathf.Min(currentRotation + rotationThisFrame, maxRotation);
            transform.RotateAround(pivotPoint.position, Vector3.back, rotationThisFrame);

            // Check if lid is now fully open
            if (currentRotation >= openThreshold && !isFullyOpen)
            {
                isFullyOpen = true;
                EnableZombiePassthrough();
            }
        }
        else if (!isOpen && currentRotation > 0)
        {
            float rotationThisFrame = lidRotationSpeed * Time.deltaTime;
            currentRotation = Mathf.Max(currentRotation - rotationThisFrame, 0);
            transform.RotateAround(pivotPoint.position, Vector3.forward, rotationThisFrame);

            // Check if lid is now closing
            if (isFullyOpen && currentRotation < openThreshold)
            {
                isFullyOpen = false;
                DisableZombiePassthrough();
            }
        }
    }

    public void ActivateRedLever()
    {
        isOpen = true;
    }

    public void DeactivateLid()
    {
        isOpen = false;
    }

    private void EnableZombiePassthrough()
    {
        if (lidCollider != null)
        {
            // Modify the collider to allow zombies to pass through
            // Option 1: Disable the collider completely
            lidCollider.enabled = false;

            // Option 2: Change collision detection for zombie-related layers
            // This requires setting up proper layers in Unity
            // Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Zombie"), true);

            Debug.Log("Furnace lid fully open - zombies can pass through");
        }
    }

    private void DisableZombiePassthrough()
    {
        if (lidCollider != null)
        {
            // Re-enable the collider to block zombies
            lidCollider.enabled = true;

            // If using layer-based approach:
            // Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Zombie"), false);

            Debug.Log("Furnace lid closed - zombies cannot pass through");
        }
    }

    // Public method to check if zombies can pass through
    public bool CanZombiesPassThrough()
    {
        return isFullyOpen;
    }

    private void OnDrawGizmos()
    {
        if (pivotPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(pivotPoint.position, 0.1f);
            Gizmos.DrawLine(pivotPoint.position, transform.position);
        }
    }
}

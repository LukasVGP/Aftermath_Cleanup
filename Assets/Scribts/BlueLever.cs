using UnityEngine;

public class BlueLever : MonoBehaviour
{
    [SerializeField] private float leverRotation = 90f;
    private Animator animator;
    private ConveyorBelt conveyorBelt;

    private void Start()
    {
        animator = GetComponent<Animator>();
        conveyorBelt = FindFirstObjectByType<ConveyorBelt>();
    }

    public void ActivateLever()
    {
        // Use the leverRotation field
        transform.rotation = Quaternion.Euler(0, 0, leverRotation);
        animator?.SetBool("IsActivated", true);
        conveyorBelt?.Activate();
    }

    public void DeactivateLever()
    {
        transform.rotation = Quaternion.Euler(0, 0, 0);
        animator?.SetBool("IsActivated", false);
        conveyorBelt?.Deactivate();
    }
}

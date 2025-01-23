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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad9))
        {
            ActivateLever();
        }
        else if (Input.GetKeyUp(KeyCode.Keypad9))
        {
            DeactivateLever();
        }
    }

    public void ActivateLever()
    {
        transform.rotation = Quaternion.Euler(0, 0, leverRotation);
        if (animator) animator.SetBool("IsActivated", true);
        if (conveyorBelt) conveyorBelt.Activate();
    }

    public void DeactivateLever()
    {
        transform.rotation = Quaternion.Euler(0, 0, 0);
        if (animator) animator.SetBool("IsActivated", false);
        if (conveyorBelt) conveyorBelt.Deactivate();
    }
}

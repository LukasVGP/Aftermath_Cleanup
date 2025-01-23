using UnityEngine;

public class BlueLever : MonoBehaviour
{
    [SerializeField] private ConveyorBelt conveyorBelt;
    [SerializeField] private float leverRotation = 45f;
    private bool isActivated = false;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void ActivateLever()
    {
        isActivated = true;
        animator.SetBool("IsActivated", true);
        transform.rotation = Quaternion.Euler(0, 0, leverRotation);
        conveyorBelt.ActivateBlueLever();
    }

    public void DeactivateLever()
    {
        isActivated = false;
        animator.SetBool("IsActivated", false);
        transform.rotation = Quaternion.Euler(0, 0, 0);
        conveyorBelt.DeactivateBelt();
    }
}

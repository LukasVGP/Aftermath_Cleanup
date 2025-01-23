using UnityEngine;

public class RedLever : MonoBehaviour
{
    [SerializeField] private FurnaceLid furnaceLid;
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
        furnaceLid.ActivateRedLever();
    }

    public void DeactivateLever()
    {
        isActivated = false;
        animator.SetBool("IsActivated", false);
        transform.rotation = Quaternion.Euler(0, 0, 0);
        furnaceLid.DeactivateLid();
    }

    public bool IsActivated()
    {
        return isActivated;
    }
}

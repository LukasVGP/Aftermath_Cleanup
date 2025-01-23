using UnityEngine;

public class FurnaceLid : MonoBehaviour
{
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float openSpeed = 2f;

    private bool isOpen = false;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void ActivateRedLever()
    {
        isOpen = true;
        animator.SetBool("IsOpen", true);
    }

    public void DeactivateLid()
    {
        isOpen = false;
        animator.SetBool("IsOpen", false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isOpen && other.TryGetComponent<ZombieBody>(out ZombieBody body))
        {
            GameManager.Instance.AddScore(100);
            Destroy(body.gameObject);
        }
    }
}

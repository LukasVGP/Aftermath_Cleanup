using UnityEngine;

public class FurnaceLid : MonoBehaviour
{
    [SerializeField] private float lidRotationSpeed = 2f;
    private float currentRotation = 0f;
    private bool isOpen = false;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isOpen && currentRotation < 90f)
        {
            currentRotation += lidRotationSpeed * Time.deltaTime;
            transform.rotation = Quaternion.Euler(0, 0, currentRotation);
        }
        else if (!isOpen && currentRotation > 0f)
        {
            currentRotation -= lidRotationSpeed * Time.deltaTime;
            transform.rotation = Quaternion.Euler(0, 0, currentRotation);
        }
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

using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    [SerializeField] private float beltSpeed = 2f;
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;

    private bool isActive = false;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isActive)
        {
            MoveItems();
        }
    }

    public void ActivateBlueLever()
    {
        isActive = true;
        animator.SetBool("IsMoving", true);
    }

    public void DeactivateBelt()
    {
        isActive = false;
        animator.SetBool("IsMoving", false);
    }

    private void MoveItems()
    {
        Collider2D[] items = Physics2D.OverlapAreaAll(startPoint.position, endPoint.position);
        foreach (Collider2D item in items)
        {
            ZombieBody body = item.GetComponent<ZombieBody>();
            if (body != null)
            {
                item.transform.Translate(Vector2.right * beltSpeed * Time.deltaTime);
            }
        }
    }
}

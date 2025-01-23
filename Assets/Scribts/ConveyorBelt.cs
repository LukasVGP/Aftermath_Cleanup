using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform endPoint;
    [SerializeField] private float beltSpeed = 2f;
    [SerializeField] private Vector2 moveDirection = Vector2.right;
    private bool isActive = false;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Activate()
    {
        isActive = true;
        animator?.SetBool("IsMoving", true);
    }

    public void Deactivate()
    {
        isActive = false;
        animator?.SetBool("IsMoving", false);
    }

    public void PlaceBodyOnBelt(ZombieBody body)
    {
        if (body != null && spawnPoint != null)
        {
            body.transform.position = spawnPoint.position;
            body.transform.parent = null;
            body.OnDropped();
        }
    }

    private void Update()
    {
        if (isActive)
        {
            MoveItems();
        }
    }

    private void MoveItems()
    {
        if (!isActive || spawnPoint == null || endPoint == null) return;

        Collider2D[] items = Physics2D.OverlapAreaAll(spawnPoint.position, endPoint.position);
        foreach (Collider2D item in items)
        {
            if (item.TryGetComponent<ZombieBody>(out var body))
            {
                item.transform.Translate(moveDirection.normalized * beltSpeed * Time.deltaTime);
            }
        }
    }
}

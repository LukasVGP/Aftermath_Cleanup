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

    public Transform GetSpawnPoint()
    {
        return spawnPoint;
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
        Collider2D[] items = Physics2D.OverlapAreaAll(spawnPoint.position, new Vector2(endPoint.position.x, endPoint.position.y));

        foreach (Collider2D item in items)
        {
            if (item.TryGetComponent(out ZombieBody body) && !body.IsBeingCarried())
            {
                Vector3 newPosition = item.transform.position + (Vector3)(moveDirection.normalized * beltSpeed * Time.deltaTime);
                item.transform.position = newPosition;

                if (Vector2.Distance(item.transform.position, endPoint.position) < 0.1f)
                {
                    Destroy(item.gameObject);
                    if (GameManager.Instance) GameManager.Instance.AddScore(100);
                }
            }
        }
    }

    public void Activate()
    {
        isActive = true;
        if (animator) animator.SetBool("IsMoving", true);
    }

    public void Deactivate()
    {
        isActive = false;
        if (animator) animator.SetBool("IsMoving", false);
    }
}

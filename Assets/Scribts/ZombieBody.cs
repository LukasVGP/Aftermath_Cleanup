using UnityEngine;

public class ZombieBody : MonoBehaviour
{
    [SerializeField] private float tearDistance = 5f;
    [SerializeField] private int fullBodyPoints = 100;
    [SerializeField] private int halfBodyPoints = 40;

    private RedPlayerController redPlayer;
    private BluePlayerController bluePlayer;
    private bool isTorn = false;
    private bool isBeingCarried = false;

    // Update the Start() method
    private void Start()
    {
        redPlayer = Object.FindFirstObjectByType<RedPlayerController>();
        bluePlayer = Object.FindFirstObjectByType<BluePlayerController>();
    }


    private void Update()
    {
        if (isBeingCarried && !isTorn)
        {
            float distance = Vector3.Distance(redPlayer.GetPosition(), bluePlayer.GetPosition());
            if (distance > tearDistance)
            {
                TearApart();
            }
        }
    }

    public void OnPickedUp(MonoBehaviour player)
    {
        isBeingCarried = true;
        transform.parent = player.transform;
    }

    public void OnDropped()
    {
        isBeingCarried = false;
        transform.parent = null;
    }

    private void TearApart()
    {
        isTorn = true;
        // Spawn two half bodies
        // Deduct points
        GameManager.Instance.DeductPoints(fullBodyPoints - (halfBodyPoints * 2));
    }

    public bool IsBeingCarried() => isBeingCarried;
}

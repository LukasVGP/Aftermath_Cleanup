using UnityEngine;

public class ZombieBody : MonoBehaviour
{
    private MonoBehaviour currentCarrier;
    private bool isBeingCarried = false;

    [Header("Grab Points")]
    public Transform headPosition;
    public Transform feetPosition;
    [SerializeField] private float grabRadius = 0.5f;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (isBeingCarried) return;

        var redPlayer = other.GetComponent<RedPlayerController>();
        var bluePlayer = other.GetComponent<BluePlayerController>();

        if (redPlayer != null && redPlayer.WantsToGrab)
        {
            CheckGrabPoints(redPlayer);
        }
        if (bluePlayer != null && bluePlayer.WantsToGrab)
        {
            CheckGrabPoints(bluePlayer);
        }
    }

    private void CheckGrabPoints(MonoBehaviour player)
    {
        float distanceToHead = Vector2.Distance(player.transform.position, headPosition.position);
        float distanceToFeet = Vector2.Distance(player.transform.position, feetPosition.position);

        if (distanceToHead <= grabRadius || distanceToFeet <= grabRadius)
        {
            TryGrab(player);
        }
    }

    private void TryGrab(MonoBehaviour player)
    {
        currentCarrier = player;
        isBeingCarried = true;
        transform.parent = player.transform;

        if (player is RedPlayerController redPlayer) redPlayer.GrabBody(this);
        if (player is BluePlayerController bluePlayer) bluePlayer.GrabBody(this);
    }

    public void Release(MonoBehaviour player)
    {
        if (player == currentCarrier)
        {
            currentCarrier = null;
            isBeingCarried = false;
            transform.parent = null;
        }
    }

    public void OnDropped()
    {
        currentCarrier = null;
        isBeingCarried = false;
        transform.parent = null;
    }

    private void Update()
    {
        if (isBeingCarried && currentCarrier != null)
        {
            transform.position = currentCarrier.transform.position;
        }
    }

    public bool IsBeingCarried() => isBeingCarried;
}

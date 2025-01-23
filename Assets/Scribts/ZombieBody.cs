using UnityEngine;

public class ZombieBody : MonoBehaviour
{
    [Header("Grab Points")]
    public Transform headPosition;
    public Transform feetPosition;

    private MonoBehaviour headPlayer;
    private MonoBehaviour feetPlayer;
    private bool isBeingCarried = false;

    private void OnTriggerStay2D(Collider2D other)
    {
        var redPlayer = other.GetComponent<RedPlayerController>();
        var bluePlayer = other.GetComponent<BluePlayerController>();

        if (redPlayer != null && redPlayer.WantsToGrab)
        {
            TryGrab(redPlayer);
        }
        else if (bluePlayer != null && bluePlayer.WantsToGrab)
        {
            TryGrab(bluePlayer);
        }
    }

    private void TryGrab(MonoBehaviour player)
    {
        float distanceToHead = Vector2.Distance(player.transform.position, headPosition.position);
        float distanceToFeet = Vector2.Distance(player.transform.position, feetPosition.position);

        if (distanceToHead < 1f && headPlayer == null)
        {
            headPlayer = player;
            if (player is RedPlayerController redPlayer) redPlayer.GrabBody(this);
            if (player is BluePlayerController bluePlayer) bluePlayer.GrabBody(this);
        }
        else if (distanceToFeet < 1f && feetPlayer == null)
        {
            feetPlayer = player;
            if (player is RedPlayerController redPlayer) redPlayer.GrabBody(this);
            if (player is BluePlayerController bluePlayer) bluePlayer.GrabBody(this);
        }

        isBeingCarried = (headPlayer != null && feetPlayer != null);
    }

    public void Release(MonoBehaviour player)
    {
        if (player == headPlayer) headPlayer = null;
        if (player == feetPlayer) feetPlayer = null;
        isBeingCarried = false;
    }

    public void OnDropped()
    {
        headPlayer = null;
        feetPlayer = null;
        isBeingCarried = false;
        transform.parent = null;
    }

    private void Update()
    {
        if (isBeingCarried)
        {
            Vector3 midPoint = (headPlayer.transform.position + feetPlayer.transform.position) / 2f;
            transform.position = midPoint;

            Vector3 direction = (headPlayer.transform.position - feetPlayer.transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    public bool IsBeingCarried() => isBeingCarried;
}

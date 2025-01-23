using UnityEngine;

public class ZombieBody : MonoBehaviour
{
    [Header("Grab Points")]
    public Transform headPosition;
    public Transform feetPosition;

    private PlayerController headPlayer;
    private PlayerController feetPlayer;
    private bool isBeingCarried = false;

    private void OnTriggerStay2D(Collider2D other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null && player.WantsToGrab)
        {
            TryGrab(player);
        }
    }

    private void TryGrab(PlayerController player)
    {
        float distanceToHead = Vector2.Distance(player.transform.position, headPosition.position);
        float distanceToFeet = Vector2.Distance(player.transform.position, feetPosition.position);

        if (distanceToHead < 1f && headPlayer == null)
        {
            headPlayer = player;
            player.GrabBody(this);
        }
        else if (distanceToFeet < 1f && feetPlayer == null)
        {
            feetPlayer = player;
            player.GrabBody(this);
        }

        isBeingCarried = (headPlayer != null && feetPlayer != null);
    }

    public void Release(PlayerController player)
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

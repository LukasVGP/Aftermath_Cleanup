using UnityEngine;

public class ZombieBody : MonoBehaviour
{
    [Header("Grab Points")]
    [Tooltip("Drag the head position empty GameObject here")]
    public Transform headPosition;
    [Tooltip("Drag the feet position empty GameObject here")]
    public Transform feetPosition;

    private RedPlayerController redCarrier;
    private BluePlayerController blueCarrier;
    private bool isBeingCarried = false;

    public void OnPickedUp(MonoBehaviour player)
    {
        if (player is RedPlayerController red && IsNearFeet(red.transform.position))
        {
            redCarrier = red;
        }
        else if (player is BluePlayerController blue && IsNearHead(blue.transform.position))
        {
            blueCarrier = blue;
        }

        isBeingCarried = (redCarrier != null && blueCarrier != null);
        if (isBeingCarried)
        {
            UpdatePosition();
        }
    }

    public void OnDropped()
    {
        redCarrier = null;
        blueCarrier = null;
        isBeingCarried = false;
        transform.parent = null;
    }

    private bool IsNearHead(Vector3 position)
    {
        return Vector3.Distance(position, headPosition.position) < 1f;
    }

    private bool IsNearFeet(Vector3 position)
    {
        return Vector3.Distance(position, feetPosition.position) < 1f;
    }

    private void UpdatePosition()
    {
        if (isBeingCarried && redCarrier != null && blueCarrier != null)
        {
            Vector3 midPoint = (redCarrier.transform.position + blueCarrier.transform.position) / 2f;
            transform.position = midPoint;

            Vector3 direction = (blueCarrier.transform.position - redCarrier.transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    private void Update()
    {
        if (isBeingCarried)
        {
            UpdatePosition();
        }
    }

    public bool IsBeingCarried()
    {
        return isBeingCarried;
    }
}

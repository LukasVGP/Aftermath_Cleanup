using UnityEngine;

public class DualPlayerCamera : MonoBehaviour
{
    [SerializeField] private Transform player1;
    [SerializeField] private Transform player2;
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);
    private float fixedYPosition;

    void Start()
    {
        fixedYPosition = transform.position.y;
    }

    void LateUpdate()
    {
        if (player1 == null || player2 == null) return;
        Vector3 centerPoint = (player1.position + player2.position) * 0.5f;
        Vector3 newPosition = new Vector3(centerPoint.x, fixedYPosition, offset.z);
        transform.position = Vector3.Lerp(transform.position, newPosition, smoothSpeed * Time.deltaTime);
    }
}

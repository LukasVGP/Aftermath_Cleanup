using UnityEngine;

public class DynamicCameraController : MonoBehaviour
{
    [Header("Players")]
    [SerializeField] private Transform player1;
    [SerializeField] private Transform player2;

    [Header("Camera Settings")]
    [SerializeField] private float smoothTime = 0.5f;
    [SerializeField] private float minSize = 5f;
    [SerializeField] private float maxSize = 10f;
    [SerializeField] private float sizeMargin = 1.5f;
    [SerializeField] private Vector2 screenMargin = new Vector2(2f, 1f);

    private Camera cam;
    private Vector3 velocity = Vector3.zero;
    private float vertExtent;
    private float horzExtent;

    private void Start()
    {
        cam = GetComponent<Camera>();
        UpdateScreenExtents();
    }

    private void LateUpdate()
    {
        UpdateScreenExtents();
        Vector3 midPoint = (player1.position + player2.position) * 0.5f;
        midPoint.z = transform.position.z;

        transform.position = Vector3.SmoothDamp(transform.position, midPoint, ref velocity, smoothTime);

        float distance = Vector3.Distance(player1.position, player2.position);
        float targetSize = Mathf.Clamp(distance * sizeMargin, minSize, maxSize);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, Time.deltaTime * 2f);

        ClampPlayersToScreen();
    }

    private void UpdateScreenExtents()
    {
        vertExtent = cam.orthographicSize;
        horzExtent = vertExtent * Screen.width / Screen.height;
    }

    private void ClampPlayersToScreen()
    {
        Vector3 p1Pos = player1.position;
        Vector3 p2Pos = player2.position;

        float leftBound = transform.position.x - horzExtent + screenMargin.x;
        float rightBound = transform.position.x + horzExtent - screenMargin.x;
        float bottomBound = transform.position.y - vertExtent + screenMargin.y;
        float topBound = transform.position.y + vertExtent - screenMargin.y;

        if (p1Pos.x < leftBound) player1.position = new Vector3(leftBound, p1Pos.y, p1Pos.z);
        if (p1Pos.x > rightBound) player1.position = new Vector3(rightBound, p1Pos.y, p1Pos.z);
        if (p1Pos.y < bottomBound) player1.position = new Vector3(p1Pos.x, bottomBound, p1Pos.z);
        if (p1Pos.y > topBound) player1.position = new Vector3(p1Pos.x, topBound, p1Pos.z);

        if (p2Pos.x < leftBound) player2.position = new Vector3(leftBound, p2Pos.y, p2Pos.z);
        if (p2Pos.x > rightBound) player2.position = new Vector3(rightBound, p2Pos.y, p2Pos.z);
        if (p2Pos.y < bottomBound) player2.position = new Vector3(p2Pos.x, bottomBound, p2Pos.z);
        if (p2Pos.y > topBound) player2.position = new Vector3(p2Pos.x, topBound, p2Pos.z);
    }
}

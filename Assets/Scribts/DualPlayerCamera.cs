using UnityEngine;

public class DualPlayerCamera : MonoBehaviour
{
    [SerializeField] private Transform player1;
    [SerializeField] private Transform player2;
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private float fixedResolutionWidth = 1920f;
    [SerializeField] private float fixedResolutionHeight = 1080f;
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);

    private Camera cam;
    private float targetAspect;
    private float initialSize;

    void Start()
    {
        cam = GetComponent<Camera>();
        targetAspect = fixedResolutionWidth / fixedResolutionHeight;
        initialSize = cam.orthographicSize;
        SetupCamera();
    }

    void SetupCamera()
    {
        float windowAspect = (float)Screen.width / (float)Screen.height;
        float scaleHeight = windowAspect / targetAspect;

        if (scaleHeight < 1.0f)
        {
            cam.orthographicSize = initialSize / scaleHeight;
        }
    }

    void LateUpdate()
    {
        if (player1 == null || player2 == null) return;
        Move();
    }

    void Move()
    {
        Vector3 centerPoint = (player1.position + player2.position) * 0.5f;
        Vector3 newPosition = centerPoint + offset;
        transform.position = Vector3.Lerp(transform.position, newPosition, smoothSpeed * Time.deltaTime);
    }
}

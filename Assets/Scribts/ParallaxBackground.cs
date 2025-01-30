using UnityEngine;

public class ParallaxBackgroundLayer : MonoBehaviour
{
    [SerializeField] private float parallaxEffect = 0.5f;
    private Transform cameraTransform;
    private Vector3 lastCameraPosition;

    private void Start()
    {
        cameraTransform = Camera.main.transform;
        lastCameraPosition = cameraTransform.position;
    }

    private void LateUpdate()
    {
        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;
        transform.position += deltaMovement * parallaxEffect;
        lastCameraPosition = cameraTransform.position;
    }
}

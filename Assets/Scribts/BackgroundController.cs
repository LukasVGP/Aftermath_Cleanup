using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float parallaxEffect = 1f;

    private void LateUpdate()
    {
        Vector3 newPosition = new Vector3(
            cameraTransform.position.x * parallaxEffect,
            transform.position.y,
            transform.position.z
        );

        transform.position = newPosition;
    }
}

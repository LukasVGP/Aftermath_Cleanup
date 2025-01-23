using UnityEngine;

public class FurnaceLid : MonoBehaviour
{
    [SerializeField] private float lidRotationSpeed = 90f;
    [SerializeField] private Transform pivotPoint;
    [SerializeField] private float maxRotation = 90f;

    private float currentRotation = 0f;
    private bool isOpen = false;

    void Update()
    {
        if (isOpen && currentRotation < maxRotation)
        {
            float rotationThisFrame = lidRotationSpeed * Time.deltaTime;
            currentRotation = Mathf.Min(currentRotation + rotationThisFrame, maxRotation);
            transform.RotateAround(pivotPoint.position, Vector3.back, rotationThisFrame);
        }
        else if (!isOpen && currentRotation > 0)
        {
            float rotationThisFrame = lidRotationSpeed * Time.deltaTime;
            currentRotation = Mathf.Max(currentRotation - rotationThisFrame, 0);
            transform.RotateAround(pivotPoint.position, Vector3.forward, rotationThisFrame);
        }
    }

    public void ActivateRedLever()
    {
        isOpen = true;
    }

    public void DeactivateLid()
    {
        isOpen = false;
    }

    private void OnDrawGizmos()
    {
        if (pivotPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(pivotPoint.position, 0.1f);
            Gizmos.DrawLine(pivotPoint.position, transform.position);
        }
    }
}

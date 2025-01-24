using UnityEngine;
using UnityEngine.UI;

public class StrainMeter : MonoBehaviour
{
    [SerializeField] private Image strainMeterBackground;
    [SerializeField] private Image arrowIndicator;
    [SerializeField] private RectTransform arrowRect;
    [SerializeField] private Vector3 offset = new Vector3(0f, 1f, 0f);

    [Header("Strain Settings")]
    [SerializeField] private float baseZombieLength = 2f;
    [SerializeField] private float maxStrainMultiplier = 2f;

    private Camera mainCamera;
    private Canvas canvas;
    private Transform target;

    private void Start()
    {
        mainCamera = Camera.main;
        canvas = GetComponentInParent<Canvas>();
        gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        if (target != null)
        {
            Vector3 worldPosition = target.position + offset;
            Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);
            transform.position = screenPosition;
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        gameObject.SetActive(target != null);
    }

    public void UpdateStrain(float currentDistance)
    {
        float normalizedDistance = (currentDistance - baseZombieLength) /
            (baseZombieLength * (maxStrainMultiplier - 1));
        float strainPercentage = Mathf.Clamp01(normalizedDistance);

        float arrowPosition = strainPercentage * strainMeterBackground.rectTransform.rect.width;
        arrowRect.anchoredPosition = new Vector2(arrowPosition, arrowRect.anchoredPosition.y);
    }
}

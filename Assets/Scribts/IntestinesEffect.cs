using UnityEngine;

public class IntestinesEffect : MonoBehaviour
{
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private float fadeSpeed = 1f;

    private SpriteRenderer spriteRenderer;
    private float currentLifetime;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }
        currentLifetime = lifetime;
    }

    private void Update()
    {
        currentLifetime -= Time.deltaTime;

        if (currentLifetime <= 0)
        {
            float alpha = spriteRenderer.color.a;
            alpha -= fadeSpeed * Time.deltaTime;
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, alpha);

            if (alpha <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}

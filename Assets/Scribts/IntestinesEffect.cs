using UnityEngine;

public class IntestinesEffect : MonoBehaviour
{
    [SerializeField] private float lifetime = 2f;
    [SerializeField] private int particleCount = 10;
    [SerializeField] private float spreadRadius = 1f;

    private void Start()
    {
        SpawnParticles();
        Destroy(gameObject, lifetime);
    }

    private void SpawnParticles()
    {
        for (int i = 0; i < particleCount; i++)
        {
            Vector2 randomDirection = Random.insideUnitCircle * spreadRadius;
            GameObject particle = GameObject.CreatePrimitive(Random.value > 0.5f ? PrimitiveType.Quad : PrimitiveType.Cube);

            if (particle.GetComponent<MeshFilter>().mesh.name.Contains("Cube"))
            {
                particle.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
            }

            particle.transform.localScale = Vector3.one * 0.2f;
            particle.transform.position = transform.position + (Vector3)randomDirection;
            particle.GetComponent<Renderer>().material.color = Color.green;

            Rigidbody2D rb = particle.AddComponent<Rigidbody2D>();
            rb.AddForce(randomDirection * 5f, ForceMode2D.Impulse);

            Destroy(particle, lifetime);
        }
    }
}

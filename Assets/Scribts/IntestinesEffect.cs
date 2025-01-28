using UnityEngine;

public class IntestinesEffect : MonoBehaviour
{
    private void Start()
    {
        // The GameObject this script is attached to will be the intestine
        gameObject.AddComponent<BoxCollider2D>();

        Rigidbody2D rb = gameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 1f;

        Destroy(gameObject, 2f);
    }
}

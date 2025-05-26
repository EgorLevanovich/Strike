using UnityEngine;

public class BallBoundaryLimiter : MonoBehaviour
{
    public float minX = -8f;
    public float maxX = 8f;
    public float minY = -4f;
    public float maxY = 4f;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        Vector2 clampedPosition = rb.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, minX, maxX);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, minY, maxY);

        if (rb.position != clampedPosition)
        {
            rb.position = clampedPosition;
            // Если хотите, чтобы мяч останавливался у края:
            // rb.velocity = Vector2.zero;
        }
    }
} 
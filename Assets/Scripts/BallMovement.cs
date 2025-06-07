using UnityEngine;

public class BallMovement : MonoBehaviour
{
    public float fallSpeed = 2f; // Скорость падения шарика
    public string passThroughTag = "Walls"; // Тег объектов, сквозь которые можно проходить
    private Rigidbody2D rb;
    private Collider2D ballCollider;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ballCollider = GetComponent<Collider2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component is missing!");
        }
        if (ballCollider == null)
        {
            Debug.LogError("Collider2D component is missing!");
        }
    }

    private void FixedUpdate()
    {
        // Движение вниз с постоянной скоростью
        if (rb != null)
        {
            rb.velocity = new Vector2(0, -fallSpeed);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Если столкнулись с объектом, который можно проходить
        if (collision.gameObject.CompareTag(passThroughTag))
        {
            Physics2D.IgnoreCollision(ballCollider, collision.collider, true);
        }
        // Если столкнулись с Ground — уничтожаем шарик
        if (collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Respawn"))
        {
            Time.timeScale = 0f;
            // Здесь можно добавить вызов меню смерти, если нужно
        }
    }
} 
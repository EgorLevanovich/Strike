using UnityEngine;

public class Onway : MonoBehaviour
{
    [Header("Настройки")]
    public GameObject[] passthroughObjects; // Объекты сквозь которые можно пройти

    private Collider2D _enemyCollider;
    private Rigidbody2D _rb;

    void Start()
    {
        _enemyCollider = GetComponent<Collider2D>();
        _rb = GetComponent<Rigidbody2D>();

        // Изначально игнорируем коллизии с указанными объектами
        foreach (var obj in passthroughObjects)
        {
            if (obj != null)
            {
                foreach (var col in obj.GetComponents<Collider2D>())
                {
                    Physics2D.IgnoreCollision(_enemyCollider, col, true);
                }
            }
        }
    }

    // Альтернативный вариант для триггеров
    void OnTriggerEnter2D(Collider2D other)
    {
        // Если нужно дополнительное поведение
    }
}
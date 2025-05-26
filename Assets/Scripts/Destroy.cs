using UnityEngine;

public class Destroy : MonoBehaviour
{
    [Tooltip("Уничтожать этот объект при столкновении")]
    public bool destroySelf = true;

    [Tooltip("Уничтожать другой объект при столкновении")]
    public bool destroyOther = true;

    [Tooltip("Теги объектов, которые нужно уничтожать (оставьте пустым для любых объектов)")]
    public string[] targetTags;
    public AudioSource AudioSource;
    private void OnCollisionEnter(Collision collision)
    {
        ProcessCollision(collision.gameObject);
    }

    // Для триггеров (если нужно)
    private void OnTriggerEnter(Collider other)
    {
        ProcessCollision(other.gameObject);
    }

    private void ProcessCollision(GameObject other)
    {
        // Проверяем теги, если они заданы
        if (targetTags.Length > 0)
        {
            bool tagMatch = false;
            foreach (string tag in targetTags)
            {
                if (other.CompareTag(tag))
                {
                    tagMatch = true;
                    break;
                }
            }
            if (!tagMatch) return;
        }

        // Уничтожаем другие объекты
        if (destroyOther)
        {
            Destroy(other.gameObject);
            AudioSource.Play();
        }

        // Уничтожаем этот объект
        if (destroySelf)
        {
            Destroy(gameObject);
        }
    }
}
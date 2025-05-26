using UnityEngine;

public class EnemyColorApplier : MonoBehaviour
{
    private void Start()
    {
        ApplySelectedColor();
    }

    private void ApplySelectedColor()
    {
        // Проверяем, был ли сохранен цвет
        if (PlayerPrefs.HasKey("EnemyColorR"))
        {
            // Восстанавливаем сохраненный цвет
            Color savedColor = new Color(
                PlayerPrefs.GetFloat("EnemyColorR"),
                PlayerPrefs.GetFloat("EnemyColorG"),
                PlayerPrefs.GetFloat("EnemyColorB"),
                PlayerPrefs.GetFloat("EnemyColorA")
            );

            // Находим всех врагов в сцене
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject enemy in enemies)
            {
                // Применяем цвет к спрайту врага
                SpriteRenderer spriteRenderer = enemy.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    spriteRenderer.color = savedColor;
                    Debug.Log($"[EnemyColorApplier] Applied color {savedColor} to enemy {enemy.name}");
                }
            }
        }
        else
        {
            Debug.Log("[EnemyColorApplier] No color selected, using default");
        }
    }
} 
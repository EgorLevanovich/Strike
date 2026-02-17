using UnityEngine;

public class EnemyColorApplier : MonoBehaviour
{
    private void Awake()
    {
        ApplySelectedColor();
    }

    private void ApplySelectedColor()
    {
        if (PlayerPrefs.HasKey("EnemyColorR"))
        {
            var savedColor = new Color(
                PlayerPrefs.GetFloat("EnemyColorR"),
                PlayerPrefs.GetFloat("EnemyColorG"),
                PlayerPrefs.GetFloat("EnemyColorB"),
                PlayerPrefs.GetFloat("EnemyColorA")
            );
            
            var enemies = Object.FindObjectsByType<EnemyPointsGiver>(FindObjectsSortMode.None);
            foreach (var enemy in enemies)
            {
                if (enemy.Renderer != null)
                {
                    enemy.Renderer .color = savedColor;
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
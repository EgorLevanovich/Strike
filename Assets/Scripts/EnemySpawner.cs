using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Настройки спавна")]
    public GameObject enemyPrefab;
    public Transform spawnContainer; //     (контейнер)

    [Header("Настройки волны")]
    public int enemyCount = 8;       // максимально 8 шариков
    public float lineLength = 10f;   // длина линии спавна
    public float spawnInterval = 8f; // интервал между спавнами
    public float yOffset = 0f;      // смещение по вертикали
    [Header("Автоматический расчёт расстояния")]
    public float extraSpacing = 0.2f; // дополнительный отступ между шарами

    // Новое поле для ссылки на BonusSystem2D
    public BonusSystem2D bonusSystem;

    // Новый флаг для отслеживания начала новой волны
    private bool isNewWave = true;

    void Start()
    {
        StartCoroutine(ContinuousSpawning());
    }

    IEnumerator ContinuousSpawning()
    {
        while (true)
        {
            isNewWave = true; // Перед каждой волной отмечаем, что это новая волна
            SpawnLine();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnLine()
    {
        float prefabWidth = 1f;
        if (enemyPrefab != null)
        {
            var sr = enemyPrefab.GetComponent<SpriteRenderer>();
            if (sr != null)
                prefabWidth = sr.bounds.size.x;
            else
            {
                var collider = enemyPrefab.GetComponent<Collider2D>();
                if (collider != null)
                    prefabWidth = collider.bounds.size.x;
            }
        }
        float spacing = prefabWidth + extraSpacing;
        float totalLength = spacing * (enemyCount - 1);
        Vector3 startPoint = transform.position - new Vector3(totalLength / 2, 0, 0);
        startPoint.y += yOffset;

        bool spawnBonus = bonusSystem != null && bonusSystem.spawnBonusInNextWave && isNewWave;
        int bonusIndex = spawnBonus ? bonusSystem.bonusIndexInWave : -1;

        Vector2 defaultVelocity = Vector2.down * 2f;
        var prefabRb = enemyPrefab.GetComponent<Rigidbody2D>();
        if (prefabRb != null)
            defaultVelocity = prefabRb.velocity;

        bool bonusSpawned = false;
        for (int i = 0; i < enemyCount; i++)
        {
            Vector3 spawnPos = startPoint + new Vector3(spacing * i, 0, 0);
            if (spawnBonus && i == bonusIndex && !bonusSpawned)
            {
                // Только бонус, без обычного шарика!
                bonusSpawned = bonusSystem.TrySpawnBonus(spawnPos, defaultVelocity);
                if (bonusSpawned)
                {
                    isNewWave = false; // После первого успешного спавна бонуса сбрасываем флаг
                }
                if (!bonusSpawned)
                {
                    // Не сбрасываем флаги, чтобы попытка повторилась в следующей волне
                    continue;
                }
                continue; // <--- Ключевой момент: не спавним обычный шарик на этом месте!
            }
            // Обычный шарик
            GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            if (spawnContainer != null)
                enemy.transform.SetParent(null);
            var rb = enemy.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.velocity = defaultVelocity;
        }
        // Если бонус был заспавнен — сбрасываем оба флага
        if (bonusSystem != null && bonusSpawned)
        {
            bonusSystem.spawnBonusInNextWave = false;
            bonusSystem.pendingBonusRequest = false;
        }
    }

    //  
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        float prefabWidth = 1f;
        if (enemyPrefab != null)
        {
            var sr = enemyPrefab.GetComponent<SpriteRenderer>();
            if (sr != null)
                prefabWidth = sr.bounds.size.x;
            else
            {
                var collider = enemyPrefab.GetComponent<Collider2D>();
                if (collider != null)
                    prefabWidth = collider.bounds.size.x;
            }
        }
        float spacing = prefabWidth + extraSpacing;
        float totalLength = spacing * (enemyCount - 1);
        Vector3 startPoint = transform.position - new Vector3(totalLength / 2, yOffset, 0);
        for (int i = 0; i < enemyCount; i++)
        {
            Vector3 point = startPoint + new Vector3(spacing * i, 0, 0);
            Gizmos.DrawWireCube(point, new Vector3(prefabWidth, prefabWidth, 0.5f));
        }
    }
}
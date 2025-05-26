using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("�������� ���������")]
    public GameObject enemyPrefab;
    public Transform spawnContainer; //     (�������������)

    [Header("��������� �����")]
    public int enemyCount = 8;       // ������������ 8 ������
    public float lineLength = 10f;   // ����� ����� �����
    public float spawnInterval = 8f; // �������� ����� ��������
    public float yOffset = 0f;      // �������� �� ���������
    [Header("Автоматический расчёт расстояния")]
    public float extraSpacing = 0.2f; // дополнительный отступ между шарами

    [Header("Система бонусов")]
    public BonusSystem2D bonusSystem;

    void Start()
    {
        StartCoroutine(ContinuousSpawning());
    }

    IEnumerator ContinuousSpawning()
    {
        while (true)
        {
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

        if (bonusSystem != null && bonusSystem.shouldSpawnBonusInNextWave)
        {
            int bonusIndex = Random.Range(0, enemyCount);
            for (int i = 0; i < enemyCount; i++)
            {
                Vector3 spawnPos = startPoint + new Vector3(spacing * i, 0, 0);
                if (i == bonusIndex)
                {
                    bonusSystem.SpawnRandomBonusAtPosition(spawnPos);
                }
                else
                {
                    GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
                    if (spawnContainer != null)
                        enemy.transform.SetParent(null);
                }
            }
            bonusSystem.shouldSpawnBonusInNextWave = false;
        }
        else
        {
            for (int i = 0; i < enemyCount; i++)
            {
                Vector3 spawnPos = startPoint + new Vector3(spacing * i, 0, 0);
                GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
                if (spawnContainer != null)
                    enemy.transform.SetParent(null);
            }
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
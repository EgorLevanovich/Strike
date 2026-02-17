using UnityEngine;
using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;

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

    [SerializeField] private Transform[] _spawnPoints = System.Array.Empty<Transform>();

    // Новый флаг для отслеживания начала новой волны
    private bool isNewWave = true;
    private CancellationTokenSource _tokenSource;

    private void Start()
    {
        StartSpawningOnRespawn(false);
    }

    public void StartSpawningOnRespawn(bool usePreload)
    {
        Cancel();
        Begin(usePreload);
    }

    public void Cancel()
    {
        if (_tokenSource != null)
        {
            _tokenSource.Cancel();
            _tokenSource.Dispose();
            _tokenSource = null;
        }
    }

    public void Begin(bool usePreload)
    {
        _tokenSource = new CancellationTokenSource();
        SpawnAsync(usePreload, _tokenSource.Token).Forget();
    }

    private async UniTask SpawnAsync(bool usePreload, CancellationToken token)
    {
        var delay = usePreload ? spawnInterval : 1f;
        await UniTask.WaitForSeconds(delay, cancellationToken: token);
        
        while (!token.IsCancellationRequested)
        {
            isNewWave = true;
            SpawnLine();
            await UniTask.WaitForSeconds(spawnInterval, cancellationToken: token);
        }
    }

    private void SpawnLine()
    {
        bool spawnBonus = bonusSystem != null && bonusSystem.spawnBonusInNextWave && isNewWave;
        int bonusIndex = spawnBonus ? bonusSystem.bonusIndexInWave : -1;

        Vector2 defaultVelocity = Vector2.down * 2f;
        var prefabRb = enemyPrefab.GetComponent<Rigidbody2D>();
        if (prefabRb != null)
            defaultVelocity = prefabRb.linearVelocity;

        bool bonusSpawned = false;
        for (int i = 0; i < _spawnPoints.Length; i++)
        {
            var point = _spawnPoints[i];
            var spawnPos = point.position;
            
            //Vector3 spawnPos = startPoint + new Vector3(spacing * i, 0, 0);
            
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
            /*if (spawnContainer != null)
                enemy.transform.SetParent(null);*/
            var rb = enemy.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.linearVelocity = defaultVelocity;
        }
        // Если бонус был заспавнен — сбрасываем оба флага
        if (bonusSystem != null && bonusSpawned)
        {
            bonusSystem.spawnBonusInNextWave = false;
            bonusSystem.pendingBonusRequest = false;
        }
    }

    //  
    private void OnDrawGizmos()
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
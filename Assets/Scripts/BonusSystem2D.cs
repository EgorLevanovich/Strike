using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public enum BonusType { CloneBall, Homing, DoublePoints }

public class BonusSystem2D : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject[] bonusPrefabs; // 0-CloneBall, 1-Homing, 2-DoublePoints
    public float minSpawnTime = 20f;
    public float maxSpawnTime = 50f;
    public Rect spawnArea;
    [Tooltip("Chances: 0-CloneBall, 1-Homing, 2-DoublePoints")]
    public float[] bonusChances = new float[3] { 0.33f, 0.33f, 0.34f };

    [Header("Clone Ball Effect")]
    public float cloneDuration = 15f;
    public float cloneOffset = 0.5f;
    public GameObject _X2;

    [Header("Homing Effect")]
    public float homingDuration = 15f;
    public float homingSpeed = 15f;
    public int maxTargets = 10;
    public GameObject _Homing;

    [Header("Double Points Effect")]
    public float doublePointsDuration = 15f;
    public GameObject _DoublePoints;

    [Header("Синхронизация размера бонуса")]
    public GameObject enemyPrefab;

    public Text bonusTimerText;

    private GameObject currentBonus;
    private bool isBonusActive = false;
    private Coroutine currentHomingEffect;
    private Coroutine currentPointsEffect;
    private bool isDoublePointsActive = false;
    private int targetsDestroyed = 0;

    [HideInInspector]
    public bool shouldSpawnBonusInNextWave = false;

    private void Start()
    {
        // Проверка настройки префабов при старте
        if (bonusPrefabs == null)
        {
            Debug.LogError("[BonusSystem2D] Bonus prefabs array is null!");
            return;
        }

        Debug.Log($"[BonusSystem2D] Initialized with {bonusPrefabs.Length} bonus prefabs:");
        for (int i = 0; i < bonusPrefabs.Length; i++)
        {
            if (bonusPrefabs[i] == null)
                Debug.LogError($"[BonusSystem2D] Prefab at index {i} is null!");
            else
                Debug.Log($"[BonusSystem2D] Prefab {i}: {bonusPrefabs[i].name}");
        }

        // Проверка UI элементов
        Debug.Log($"[BonusSystem2D] UI Elements status:");
        Debug.Log($"- _X2: {(_X2 != null ? "Assigned" : "Missing")}");
        Debug.Log($"- _Homing: {(_Homing != null ? "Assigned" : "Missing")}");
        Debug.Log($"- _DoublePoints: {(_DoublePoints != null ? "Assigned" : "Missing")}");

        if (bonusTimerText != null)
            bonusTimerText.gameObject.SetActive(false);
        StartCoroutine(BonusSpawnCycle());
    }

    IEnumerator BonusSpawnCycle()
    {
        while (true)
        {
            float waitTime = Random.Range(minSpawnTime, maxSpawnTime);
            yield return new WaitForSeconds(waitTime);
            shouldSpawnBonusInNextWave = true;
            Debug.Log("[BonusSystem2D] Бонус появится в следующей волне!");
            while (shouldSpawnBonusInNextWave)
                yield return null;
        }
    }

    void SpawnRandomBonus()
    {
        // Проверка массива префабов
        if (bonusPrefabs == null || bonusPrefabs.Length < 3)
        {
            Debug.LogError("[BonusSystem2D] Not enough bonus prefabs assigned! Expected 3, got: " + 
                          (bonusPrefabs == null ? "null" : bonusPrefabs.Length.ToString()));
            return;
        }

        // Проверяем компоненты всех префабов
        for (int i = 0; i < bonusPrefabs.Length; i++)
        {
            if (bonusPrefabs[i] != null)
            {
                var sprite = bonusPrefabs[i].GetComponent<SpriteRenderer>();
                var collider = bonusPrefabs[i].GetComponent<Collider2D>();
                var rb = bonusPrefabs[i].GetComponent<Rigidbody2D>();
                var prefabController = bonusPrefabs[i].GetComponent<BonusController2D>();

                Debug.Log($"[BonusSystem2D] Prefab {(BonusType)i} components:");
                Debug.Log($"- SpriteRenderer: {(sprite != null ? "Present" : "Missing")}");
                Debug.Log($"- Collider2D: {(collider != null ? $"Present, isTrigger={collider.isTrigger}" : "Missing")}");
                Debug.Log($"- Rigidbody2D: {(rb != null ? $"Present, type={rb.bodyType}" : "Missing")}");
                Debug.Log($"- BonusController2D: {(prefabController != null ? "Already Present!" : "Not present")}");
            }
        }

        float randomValue = Random.value;
        float cumulative = 0;
        BonusType bonusType = BonusType.CloneBall;

        Debug.Log($"[BonusSystem2D] Rolling for bonus. Random value: {randomValue}");
        for (int i = 0; i < bonusChances.Length; i++)
        {
            cumulative += bonusChances[i];
            Debug.Log($"[BonusSystem2D] Bonus type {(BonusType)i}: Chance {bonusChances[i]}, Cumulative {cumulative}");
            if (randomValue <= cumulative)
            {
                bonusType = (BonusType)i;
                break;
            }
        }

        Debug.Log($"[BonusSystem2D] Selected bonus type: {bonusType}");

        Vector2 spawnPosition = new Vector2(
            Random.Range(spawnArea.xMin, spawnArea.xMax),
            Random.Range(spawnArea.yMin, spawnArea.yMax)
        );

        currentBonus = Instantiate(bonusPrefabs[(int)bonusType], spawnPosition, Quaternion.identity);
        
        // Проверяем компоненты созданного бонуса
        var spawnedSprite = currentBonus.GetComponent<SpriteRenderer>();
        var spawnedCollider = currentBonus.GetComponent<Collider2D>();
        var spawnedRb = currentBonus.GetComponent<Rigidbody2D>();
        var spawnedController = currentBonus.GetComponent<BonusController2D>();

        Debug.Log($"[BonusSystem2D] Spawned bonus components:");
        Debug.Log($"- SpriteRenderer: {(spawnedSprite != null ? "Present" : "Missing")}");
        Debug.Log($"- Collider2D: {(spawnedCollider != null ? $"Present, isTrigger={spawnedCollider.isTrigger}" : "Missing")}");
        Debug.Log($"- Rigidbody2D: {(spawnedRb != null ? $"Present, type={spawnedRb.bodyType}" : "Missing")}");
        Debug.Log($"- BonusController2D: {(spawnedController != null ? "Already Present!" : "Not present")}");

        isBonusActive = true;

        // Используем существующий контроллер или добавляем новый
        BonusController2D controller;
        if (spawnedController != null)
        {
            controller = spawnedController;
            Debug.Log("[BonusSystem2D] Using existing BonusController2D");
        }
        else
        {
            controller = currentBonus.AddComponent<BonusController2D>();
            Debug.Log("[BonusSystem2D] Added new BonusController2D");
        }
        
        controller.Setup(this, bonusType);

        Debug.Log($"[BonusSystem2D] Spawned {bonusType} bonus at position {spawnPosition}");
    }

    public void OnBonusCollected(GameObject collectorBall, BonusType bonusType)
    {
        if (currentBonus == null) 
        {
            Debug.LogError("[BonusSystem2D] OnBonusCollected called but currentBonus is null!");
            return;
        }

        Debug.Log($"[BonusSystem2D] Collected bonus: {bonusType}");
        Debug.Log($"[BonusSystem2D] Collector ball position: {collectorBall.transform.position}");
        Debug.Log($"[BonusSystem2D] Bonus position: {currentBonus.transform.position}");

        isBonusActive = false;
        Destroy(currentBonus);
        currentBonus = null;

        switch (bonusType)
        {
            case BonusType.CloneBall:
                Debug.Log("CloneBall bonus activated!");
                CreateClone(collectorBall);
                break;

            case BonusType.Homing:
                Debug.Log("Homing bonus activated!");
                ApplyHomingEffect(collectorBall);
                break;

            case BonusType.DoublePoints:
                Debug.Log("DoublePoints bonus activated!");
                ApplyPointsEffect(doublePointsDuration);
                break;
        }
    }

    void CreateClone(GameObject originalBall)
    {
        _X2.SetActive(true);
        Vector2 spawnPos = (Vector2)originalBall.transform.position +
                         (Random.insideUnitCircle.normalized * cloneOffset);
        GameObject newBall = Instantiate(originalBall, spawnPos, Quaternion.identity);
        newBall.transform.localScale = originalBall.transform.localScale;

        // Копируем скорость
        Rigidbody2D origRb = originalBall.GetComponent<Rigidbody2D>();
        Rigidbody2D cloneRb = newBall.GetComponent<Rigidbody2D>();
        if (origRb != null && cloneRb != null)
            cloneRb.velocity = origRb.velocity;

        Destroy(newBall, cloneDuration);
        StartCoroutine(CloneBonusTimerCoroutine(cloneDuration));
    }

    IEnumerator CloneBonusTimerCoroutine(float duration)
    {
        if (bonusTimerText != null)
            bonusTimerText.gameObject.SetActive(true);
        float startTime = Time.time;
        while (Time.time - startTime < duration)
        {
            float timeLeft = Mathf.Max(0, duration - (Time.time - startTime));
            if (bonusTimerText != null)
                bonusTimerText.text = $"{(int)timeLeft}";
            yield return null;
        }
        if (bonusTimerText != null)
            bonusTimerText.gameObject.SetActive(false);
        StartCoroutine(BonusSpawnCycle()); // запуск следующего бонуса только после окончания таймера
    }

    void ApplyHomingEffect(GameObject ball)
    {
        if (currentHomingEffect != null)
            StopCoroutine(currentHomingEffect);

        targetsDestroyed = 0;
        currentHomingEffect = StartCoroutine(RunHomingEffect(ball));
    }

    IEnumerator RunHomingEffect(GameObject ball)
    {
        if (_Homing != null)
            _Homing.SetActive(true);
        if (bonusTimerText != null)
            bonusTimerText.gameObject.SetActive(true);

        Rigidbody2D ballRb = ball.GetComponent<Rigidbody2D>();
        if (ballRb == null)
        {
            Debug.LogError("[BonusSystem2D] Ball doesn't have Rigidbody2D component!");
            if (bonusTimerText != null) bonusTimerText.gameObject.SetActive(false);
            yield break;
        }

        float startTime = Time.time;
        Vector2 originalVelocity = ballRb.velocity;
        float originalSpeed = originalVelocity.magnitude;
        float destroyRadius = 1f;

        while (Time.time - startTime < homingDuration && targetsDestroyed < maxTargets)
        {
            float timeLeft = Mathf.Max(0, homingDuration - (Time.time - startTime));
            if (bonusTimerText != null)
                bonusTimerText.text = $"{(int)timeLeft}";
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            bool destroyedAny = false;

            // Собираем всех врагов в радиусе
            List<GameObject> closeEnemies = new List<GameObject>();
            foreach (GameObject enemy in enemies)
            {
                if (enemy != null)
                {
                    float distance = Vector2.Distance(ball.transform.position, enemy.transform.position);
                    float enemyY = enemy.transform.position.y;
                    if (distance < destroyRadius && enemyY <= 2391f)
                        closeEnemies.Add(enemy);
                }
            }
            int leftToDestroy = maxTargets - targetsDestroyed;
            for (int i = 0; i < closeEnemies.Count; i++)
            {
                if (targetsDestroyed >= maxTargets)
                    break;
                Destroy(closeEnemies[i]);
                targetsDestroyed++;
                destroyedAny = true;
                Debug.Log($"[BonusSystem2D] Destroyed enemy! Total: {targetsDestroyed}/{maxTargets}");
            }
            if (targetsDestroyed >= maxTargets)
            {
                ballRb.velocity = new Vector2(0, -originalSpeed); // строго вниз
                if (_Homing != null)
                    _Homing.SetActive(false);
                currentHomingEffect = null;
                Debug.Log("[BonusSystem2D] Homing effect completed");
                StartCoroutine(BonusSpawnCycle()); // Запуск следующего бонуса
                yield break;
            }

            if (!destroyedAny)
            {
                // Если рядом никого нет — ищем ближайшего и летим к нему
                GameObject nearestEnemy = null;
                float minDistance = float.MaxValue;
                foreach (GameObject enemy in enemies)
                {
                    if (enemy != null)
                    {
                        float distance = Vector2.Distance(ball.transform.position, enemy.transform.position);
                        float enemyY = enemy.transform.position.y;
                        if (enemyY <= 2391f && distance < minDistance)
                        {
                            minDistance = distance;
                            nearestEnemy = enemy;
                        }
                    }
                }

                if (nearestEnemy != null)
                {
                    Vector2 direction = (nearestEnemy.transform.position - ball.transform.position).normalized;
                    ballRb.velocity = direction * homingSpeed;
                }
                else
                {
                    // Если врагов нет, продолжаем движение с исходной скоростью
                    ballRb.velocity = originalVelocity.normalized * originalSpeed;
                }
            }
            // Если кого-то уничтожили — не меняем направление, чтобы не улетать далеко

            yield return new WaitForFixedUpdate();
        }

        if (bonusTimerText != null)
            bonusTimerText.gameObject.SetActive(false);
        EndHoming:
        // Возвращаем мяч к нормальному движению
        ballRb.velocity = originalVelocity.normalized * originalSpeed;

        if (_Homing != null)
            _Homing.SetActive(false);

        currentHomingEffect = null;
        Debug.Log("[BonusSystem2D] Homing effect completed");
        StartCoroutine(BonusSpawnCycle()); // Запуск следующего бонуса
        yield break;
    }

    void ApplyPointsEffect(float duration)
    {
        if (currentPointsEffect != null)
        {
            Debug.Log("[BonusSystem2D] Stopping previous Double Points effect");
            StopCoroutine(currentPointsEffect);
        }

        Debug.Log("[BonusSystem2D] Activating Double Points!");
        currentPointsEffect = StartCoroutine(RunPointsEffect(duration));
    }

    IEnumerator RunPointsEffect(float duration)
    {
        Debug.Log("[BonusSystem2D] Starting Double Points effect");
        if (bonusTimerText != null)
            bonusTimerText.gameObject.SetActive(true);
        
        if (_DoublePoints != null)
            _DoublePoints.SetActive(true);
        else
            Debug.LogWarning("[BonusSystem2D] _DoublePoints UI object is not assigned!");

        isDoublePointsActive = true;
        if (NewBehaviourScript.Instance != null)
        {
            NewBehaviourScript.Instance.SetScoreMultiplier(2);
            Debug.Log("[BonusSystem2D] Score multiplier set to 2");
        }
        else
        {
            Debug.LogWarning("[BonusSystem2D] NewBehaviourScript.Instance is null! (set 2)");
        }

        float startTime = Time.time;
        while (Time.time - startTime < duration)
        {
            float timeLeft = Mathf.Max(0, duration - (Time.time - startTime));
            if (bonusTimerText != null)
                bonusTimerText.text = $"{(int)timeLeft}";
            yield return null;
        }

        if (bonusTimerText != null)
            bonusTimerText.gameObject.SetActive(false);

        isDoublePointsActive = false;
        if (NewBehaviourScript.Instance != null)
        {
            NewBehaviourScript.Instance.SetScoreMultiplier(1);
            Debug.Log("[BonusSystem2D] Double Points effect ended, score multiplier reset to 1");
        }
        else
        {
            Debug.LogWarning("[BonusSystem2D] NewBehaviourScript.Instance is null! (reset 1)");
        }

        if (_DoublePoints != null)
            _DoublePoints.SetActive(false);

        currentPointsEffect = null;
        StartCoroutine(BonusSpawnCycle()); // Запуск следующего бонуса
    }

    public void SpawnRandomBonusAtPosition(Vector2 position)
    {
        // Выбор типа бонуса (аналогично SpawnRandomBonus)
        float randomValue = Random.value;
        float cumulative = 0;
        BonusType bonusType = BonusType.CloneBall;
        for (int i = 0; i < bonusChances.Length; i++)
        {
            cumulative += bonusChances[i];
            if (randomValue <= cumulative)
            {
                bonusType = (BonusType)i;
                break;
            }
        }
        currentBonus = Instantiate(bonusPrefabs[(int)bonusType], position, Quaternion.identity);
        // Синхронизировать размер бонуса с enemyPrefab
        if (enemyPrefab != null)
            currentBonus.transform.localScale = enemyPrefab.transform.localScale;
        var controller = currentBonus.GetComponent<BonusController2D>();
        if (controller == null)
            controller = currentBonus.AddComponent<BonusController2D>();
        controller.Setup(this, bonusType);
        isBonusActive = true;
        Debug.Log($"[BonusSystem2D] Spawned {bonusType} bonus at position {position}");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(spawnArea.center, spawnArea.size);
    }
}
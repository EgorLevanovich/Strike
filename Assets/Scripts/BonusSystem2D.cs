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
    public Transform bonusSpawnPoint; // Точка спавна бонусов

    [Header("Bonus Colors")]
    [Tooltip("Colors for bonuses: 0-CloneBall, 1-Homing, 2-DoublePoints")]
    public Color[] bonusColors = new Color[3] { Color.white, Color.white, Color.white };

    [Header("Clone Ball Effect")]
    public float cloneDuration = 15f;
    public float cloneOffset = 0.5f;
    public GameObject _X2;

    [Header("Homing Effect")]
    public float homingDuration = 15f;
    public float homingSpeed = 15f;
    public float homingRotationSpeed = 360f; // Скорость вращения мяча в градусах в секунду
    public int maxTargets = 10;
    public GameObject _Homing;

    [Header("Double Points Effect")]
    public float doublePointsDuration = 15f;
    public GameObject _DoublePoints;

    public Text bonusTimerText;

    private GameObject currentBonus;
    private bool isBonusActive = false;
    private Coroutine currentHomingEffect;
    private Coroutine currentPointsEffect;
    private bool isDoublePointsActive = false;
    private int targetsDestroyed = 0;
    private Coroutine bonusSpawnCoroutine;
    [HideInInspector] public bool isAnyEffectActive = false; // Флаг для отслеживания активных эффектов
    private Coroutine currentCloneEffect;

    [HideInInspector] public bool spawnBonusInNextWave = false;
    [HideInInspector] public int bonusIndexInWave = -1;
    // Ссылка на EnemySpawner
    public EnemySpawner enemySpawner;

    // Новый публичный префаб для выравнивания размера бонуса
    public GameObject referenceEnemyPrefab;

    [HideInInspector] public bool pendingBonusRequest = false;

    private bool firstBonusSpawned = false; // Новый флаг для отслеживания первого бонуса

    private List<GameObject> spawnedClones = new List<GameObject>(); // Список созданных этим скриптом клонов

    [Header("Audio")]
    public AudioSource bonusActivateAudio; // Аудио для активации бонуса

    private void Start()
    {
        // Проверка настройки префабов при старте
        if (bonusPrefabs == null)
        {
            return;
        }

        if (bonusTimerText != null)
            bonusTimerText.gameObject.SetActive(false);
        if (bonusSpawnCoroutine == null)
            bonusSpawnCoroutine = StartCoroutine(BonusSpawnCycle());
    }

    private IEnumerator BonusSpawnCycle()
    {
        while (true)
        {
            // Ждем, пока текущий бонус не будет собран и все эффекты не закончатся
            while (isBonusActive || isAnyEffectActive)
            {
                yield return null;
            }

            // Ждем случайное время перед спавном нового бонуса
            float waitTime = Random.Range(minSpawnTime, maxSpawnTime);
            yield return new WaitForSeconds(waitTime);

            // Строгая защита: бонус можно только запросить на следующую волну!
            if (!isBonusActive && !isAnyEffectActive && enemySpawner != null)
            {
                if (!spawnBonusInNextWave) // Не дублируем запрос
                {
                    RequestBonusInNextWave(enemySpawner.enemyCount);
                }
            }
        }
    }

    private void SpawnRandomBonus()
    {
        if (isBonusActive || currentBonus != null || isAnyEffectActive)
        {
            return;
        }

        float randomValue = Random.value;
        float cumulative = 0;
        BonusType bonusType = BonusType.CloneBall;

        // Если это первый бонус — не даём Homing (индекс 1)
        int[] allowedFirstBonuses = new int[] { 0, 2 }; // 0 - CloneBall, 2 - DoublePoints
        if (!firstBonusSpawned)
        {
            // Выбираем только из разрешённых
            int idx = allowedFirstBonuses[Random.Range(0, allowedFirstBonuses.Length)];
            bonusType = (BonusType)idx;
            firstBonusSpawned = true;
        }
        else
        {
            for (int i = 0; i < bonusChances.Length; i++)
            {
                cumulative += bonusChances[i];
                if (randomValue <= cumulative)
                {
                    bonusType = (BonusType)i;
                    break;
                }
            }
        }

        Vector2 spawnPosition;
        if (bonusSpawnPoint != null)
        {
            spawnPosition = bonusSpawnPoint.position;
        }
        else
        {
            spawnPosition = new Vector2(
                Random.Range(spawnArea.xMin, spawnArea.xMax),
                Random.Range(spawnArea.yMin, spawnArea.yMax)
            );
        }

        currentBonus = Instantiate(bonusPrefabs[(int)bonusType], spawnPosition, Quaternion.identity);
        
        // Apply color based on bonus type
        var spriteRenderer = currentBonus.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && (int)bonusType < bonusColors.Length)
            spriteRenderer.color = bonusColors[(int)bonusType];
        
        // Проверяем компоненты созданного бонуса
        var spawnedSprite = currentBonus.GetComponent<SpriteRenderer>();
        var spawnedCollider = currentBonus.GetComponent<Collider2D>();
        var spawnedRb = currentBonus.GetComponent<Rigidbody2D>();
        var spawnedController = currentBonus.GetComponent<BonusController2D>();

        isBonusActive = true;

        // Используем существующий контроллер или добавляем новый
        BonusController2D controller;
        if (spawnedController != null)
        {
            controller = spawnedController;
        }
        else
        {
            controller = currentBonus.AddComponent<BonusController2D>();
        }
        
        controller.Setup(this, bonusType);

        // Жёстко выставить позицию до установки velocity и параметров Rigidbody2D
        currentBonus.transform.position = spawnPosition;
        currentBonus.tag = "Bonus";

        // Устанавливаем скорость и параметры Rigidbody2D
        var bonusRb = currentBonus.GetComponent<Rigidbody2D>();
        var refRb = referenceEnemyPrefab != null ? referenceEnemyPrefab.GetComponent<Rigidbody2D>() : null;
        if (bonusRb != null && refRb != null)
        {
            bonusRb.velocity = refRb.velocity;
            bonusRb.gravityScale = refRb.gravityScale;
            bonusRb.drag = refRb.drag;
            bonusRb.angularDrag = refRb.angularDrag;
            bonusRb.bodyType = refRb.bodyType;
            bonusRb.interpolation = refRb.interpolation;
            bonusRb.collisionDetectionMode = refRb.collisionDetectionMode;
            bonusRb.constraints = refRb.constraints;
            StartCoroutine(ForceVelocity(bonusRb, refRb.velocity));
        }
        else if (bonusRb != null)
        {
            bonusRb.velocity = refRb.velocity;
            StartCoroutine(ForceVelocity(bonusRb, refRb.velocity));
        }
    }

    public void OnBonusCollected(GameObject collectorBall, BonusType bonusType)
    {
        if (currentBonus == null) 
        {
            return;
        }

        isBonusActive = false;
        Destroy(currentBonus);
        currentBonus = null;

        // Воспроизводим звук активации бонуса
        if (bonusActivateAudio != null)
            bonusActivateAudio.Play();

        // Устанавливаем флаг активного эффекта
        isAnyEffectActive = true;

        switch (bonusType)
        {
            case BonusType.CloneBall:
                CreateClone(collectorBall);
                break;
            case BonusType.Homing:
                ApplyHomingEffect(collectorBall);
                break;
            case BonusType.DoublePoints:
                ApplyPointsEffect(doublePointsDuration);
                break;
        }
    }

    void CreateClone(GameObject originalBall)
    {
        if (originalBall == null)
        {
            isAnyEffectActive = false;
            return;
        }

        Vector2 spawnPos = (Vector2)originalBall.transform.position + (Random.insideUnitCircle.normalized * cloneOffset);
        GameObject newBall = Instantiate(originalBall, spawnPos, Quaternion.identity);

        if (newBall == null)
        {
            isAnyEffectActive = false;
            return;
        }

        newBall.tag = "CloneBall";
        newBall.transform.localScale = originalBall.transform.localScale;

        Rigidbody2D origRb = originalBall.GetComponent<Rigidbody2D>();
        Rigidbody2D cloneRb = newBall.GetComponent<Rigidbody2D>();
        if (origRb != null && cloneRb != null)
        {
            float speed = origRb.velocity.magnitude;
            if (speed < 1f) speed = 10f;
            Vector2 direction = Random.insideUnitCircle.normalized;
            origRb.velocity = direction * speed;
            cloneRb.velocity = -direction * speed;
        }

        spawnedClones.Add(newBall); // Добавляем клон в список

        StartCloneEffect(cloneDuration);

        StartCoroutine(WaitForCloneEffect(cloneDuration));
        Destroy(newBall, cloneDuration);
    }

    private IEnumerator WaitForCloneEffect(float duration)
    {
        yield return new WaitForSeconds(duration);
        isAnyEffectActive = false;
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
        {
            bonusTimerText.gameObject.SetActive(true);
            bonusTimerText.text = homingDuration.ToString("F1"); // Показываем время с одним знаком после запятой
        }

        Rigidbody2D ballRb = ball.GetComponent<Rigidbody2D>();
        if (ballRb == null)
        {
            if (bonusTimerText != null) bonusTimerText.gameObject.SetActive(false);
            isAnyEffectActive = false;
            yield break;
        }

        float startTime = Time.time;
        float endTime = startTime + homingDuration;
        Vector2 originalVelocity = ballRb.velocity;
        float originalSpeed = originalVelocity.magnitude;
        float originalAngularVelocity = ballRb.angularVelocity; // Сохраняем оригинальное вращение
        float destroyRadius = 1f;

        // Проверяем, является ли этот мяч мячом с индексом 10
        bool shouldRotate = false;
        
        // Находим BallSkinLoader и проверяем, какой мяч активен
        BallSkinLoader ballLoader = FindObjectOfType<BallSkinLoader>();
        if (ballLoader != null && ballLoader.ballPrefabs != null && ballLoader.ballPrefabs.Length > 10)
        {
            // Проверяем, активен ли мяч с индексом 10 (11-й элемент массива)
            if (ballLoader.ballPrefabs[10] != null && ballLoader.ballPrefabs[10].activeInHierarchy)
            {
                shouldRotate = true;
                Debug.Log("[BonusSystem2D] Мяч с индексом 10 активен, включаем вращение");
            }
        }
        
        // Начинаем вращение мяча только если это мяч с индексом 10
        if (shouldRotate)
        {
            ballRb.angularVelocity = homingRotationSpeed;
        }

        while (Time.time < endTime && targetsDestroyed < maxTargets)
        {
            float timeLeft = endTime - Time.time;
            if (bonusTimerText != null)
                bonusTimerText.text = timeLeft.ToString("F1"); // Показываем время с одним знаком после запятой
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            bool destroyedAny = false;

            // Поддерживаем вращение мяча с индексом 10 во время действия бонуса
            if (shouldRotate && ballRb != null)
            {
                ballRb.angularVelocity = homingRotationSpeed;
            }

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
            }
            if (targetsDestroyed >= maxTargets)
            {
                ballRb.velocity = new Vector2(0, -originalSpeed); // строго вниз
                if (shouldRotate)
                {
                    ballRb.angularVelocity = originalAngularVelocity; // Восстанавливаем оригинальное вращение
                }
                if (_Homing != null)
                    _Homing.SetActive(false);
                currentHomingEffect = null;
                isAnyEffectActive = false;
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

            yield return new WaitForFixedUpdate();
        }

        if (bonusTimerText != null)
            bonusTimerText.gameObject.SetActive(false);

        // Возвращаем мяч к нормальному движению
        ballRb.velocity = originalVelocity.normalized * originalSpeed;
        if (shouldRotate)
        {
            ballRb.angularVelocity = originalAngularVelocity; // Восстанавливаем оригинальное вращение
        }

        if (_Homing != null)
            _Homing.SetActive(false);

        currentHomingEffect = null;
        isAnyEffectActive = false;
    }

    void ApplyPointsEffect(float duration)
    {
        if (currentPointsEffect != null)
        {
            StopCoroutine(currentPointsEffect);
        }

        currentPointsEffect = StartCoroutine(RunPointsEffect(duration));
    }

    IEnumerator RunPointsEffect(float duration)
    {
        Debug.Log("[BonusSystem2D] Starting Double Points effect");
        if (bonusTimerText != null)
        {
            bonusTimerText.gameObject.SetActive(true);
            bonusTimerText.text = duration.ToString("F1"); // Показываем время с одним знаком после запятой
        }
        if (_DoublePoints != null)
            _DoublePoints.SetActive(true);

        isDoublePointsActive = true;
        if (NewBehaviourScript.Instance != null)
        {
            NewBehaviourScript.Instance.SetScoreMultiplier(2);
        }

        float startTime = Time.time;
        float endTime = startTime + duration;
        while (Time.time < endTime)
        {
            float timeLeft = endTime - Time.time;
            if (bonusTimerText != null)
                bonusTimerText.text = timeLeft.ToString("F1"); // Показываем время с одним знаком после запятой
            yield return null;
        }

        if (bonusTimerText != null)
            bonusTimerText.gameObject.SetActive(false);

        isDoublePointsActive = false;
        if (NewBehaviourScript.Instance != null)
        {
            NewBehaviourScript.Instance.SetScoreMultiplier(1);
        }

        if (_DoublePoints != null)
            _DoublePoints.SetActive(false);

        currentPointsEffect = null;
        isAnyEffectActive = false;
    }

    private void SpawnRandomBonusAtPosition(Vector3 position, Vector2 velocity)
    {
        // Восстановлены проверки, чтобы бонус не спавнился отдельно от волны и не появлялся, если уже есть активный бонус или эффект
        if (isBonusActive || currentBonus != null || isAnyEffectActive)
        {
            return;
        }
        float randomValue = Random.value;
        float cumulative = 0;
        BonusType bonusType = BonusType.CloneBall;

        // Если это первый бонус — не даём Homing (индекс 1)
        int[] allowedFirstBonuses = new int[] { 0, 2 }; // 0 - CloneBall, 2 - DoublePoints
        if (!firstBonusSpawned)
        {
            // Выбираем только из разрешённых
            int idx = allowedFirstBonuses[Random.Range(0, allowedFirstBonuses.Length)];
            bonusType = (BonusType)idx;
            firstBonusSpawned = true;
        }
        else
        {
            for (int i = 0; i < bonusChances.Length; i++)
            {
                cumulative += bonusChances[i];
                if (randomValue <= cumulative)
                {
                    bonusType = (BonusType)i;
                    break;
                }
            }
        }

        currentBonus = Instantiate(bonusPrefabs[(int)bonusType], position, Quaternion.identity);
        if (referenceEnemyPrefab != null)
        {
            currentBonus.transform.localScale = referenceEnemyPrefab.transform.localScale;
        }
        // Жёстко выставить позицию до установки velocity и параметров Rigidbody2D
        currentBonus.transform.position = position;
        currentBonus.tag = "Bonus";

        // Теперь velocity и параметры Rigidbody2D
        var bonusRb = currentBonus.GetComponent<Rigidbody2D>();
        var refRb = referenceEnemyPrefab != null ? referenceEnemyPrefab.GetComponent<Rigidbody2D>() : null;
        if (bonusRb != null && refRb != null)
        {
            bonusRb.velocity = velocity;
            bonusRb.gravityScale = refRb.gravityScale;
            bonusRb.drag = refRb.drag;
            bonusRb.angularDrag = refRb.angularDrag;
            bonusRb.bodyType = refRb.bodyType;
            bonusRb.interpolation = refRb.interpolation;
            bonusRb.collisionDetectionMode = refRb.collisionDetectionMode;
            bonusRb.constraints = refRb.constraints;
            StartCoroutine(ForceVelocity(bonusRb, velocity));
        }
        else if (bonusRb != null)
        {
            bonusRb.velocity = velocity;
            StartCoroutine(ForceVelocity(bonusRb, velocity));
        }
        var controller = currentBonus.GetComponent<BonusController2D>();
        if (controller == null)
            controller = currentBonus.AddComponent<BonusController2D>();
        controller.Setup(this, bonusType);
        isBonusActive = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(spawnArea.center, spawnArea.size);
    }

    // Новый метод: запросить спавн бонуса в следующей волне
    public void RequestBonusInNextWave(int count)
    {
        spawnBonusInNextWave = true;
        bonusIndexInWave = Random.Range(0, count);
    }

    public bool TrySpawnBonus(Vector3 position, Vector2 velocity)
    {
        if (isBonusActive || currentBonus != null || isAnyEffectActive)
        {
            pendingBonusRequest = true;
            return false;
        }
        pendingBonusRequest = false;
        float randomValue = Random.value;
        float cumulative = 0;
        BonusType bonusType = BonusType.CloneBall;

        // Если это первый бонус — не даём Homing (индекс 1)
        int[] allowedFirstBonuses = new int[] { 0, 2 }; // 0 - CloneBall, 2 - DoublePoints
        if (!firstBonusSpawned)
        {
            // Выбираем только из разрешённых
            int idx = allowedFirstBonuses[Random.Range(0, allowedFirstBonuses.Length)];
            bonusType = (BonusType)idx;
            firstBonusSpawned = true;
        }
        else
        {
            for (int i = 0; i < bonusChances.Length; i++)
            {
                cumulative += bonusChances[i];
                if (randomValue <= cumulative)
                {
                    bonusType = (BonusType)i;
                    break;
                }
            }
        }

        currentBonus = Instantiate(bonusPrefabs[(int)bonusType], position, Quaternion.identity);
        
        // Apply color based on bonus type
        var spriteRenderer = currentBonus.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && (int)bonusType < bonusColors.Length)
            spriteRenderer.color = bonusColors[(int)bonusType];
        
        if (referenceEnemyPrefab != null)
        {
            currentBonus.transform.localScale = referenceEnemyPrefab.transform.localScale;
        }
        // Жёстко выставить позицию до установки velocity и параметров Rigidbody2D
        currentBonus.transform.position = position;
        currentBonus.tag = "Bonus";

        // Теперь velocity и параметры Rigidbody2D
        var bonusRb = currentBonus.GetComponent<Rigidbody2D>();
        var refRb = referenceEnemyPrefab != null ? referenceEnemyPrefab.GetComponent<Rigidbody2D>() : null;
        if (bonusRb != null && refRb != null)
        {
            bonusRb.velocity = velocity;
            bonusRb.gravityScale = refRb.gravityScale;
            bonusRb.drag = refRb.drag;
            bonusRb.angularDrag = refRb.angularDrag;
            bonusRb.bodyType = refRb.bodyType;
            bonusRb.interpolation = refRb.interpolation;
            bonusRb.collisionDetectionMode = refRb.collisionDetectionMode;
            bonusRb.constraints = refRb.constraints;
            StartCoroutine(ForceVelocity(bonusRb, velocity));
        }
        else if (bonusRb != null)
        {
            bonusRb.velocity = velocity;
            StartCoroutine(ForceVelocity(bonusRb, velocity));
        }
        var controller = currentBonus.GetComponent<BonusController2D>();
        if (controller == null)
            controller = currentBonus.AddComponent<BonusController2D>();
        controller.Setup(this, bonusType);
        isBonusActive = true;
        return true;
    }

    private IEnumerator ForceVelocity(Rigidbody2D rb, Vector2 velocity)
    {
        yield return null;
        if (rb != null) rb.velocity = velocity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }

    public void StartCloneEffect(float duration)
    {
        if (currentCloneEffect != null)
            StopCoroutine(currentCloneEffect);
        currentCloneEffect = StartCoroutine(RunCloneEffect(duration));
    }

    IEnumerator RunCloneEffect(float duration)
    {
        if (bonusTimerText != null)
        {
            bonusTimerText.gameObject.SetActive(true);
            bonusTimerText.text = duration.ToString("F1"); // Показываем время с одним знаком после запятой
        }
        float startTime = Time.time;
        float endTime = startTime + duration;
        while (Time.time < endTime)
        {
            float timeLeft = endTime - Time.time;
            if (bonusTimerText != null)
                bonusTimerText.text = timeLeft.ToString("F1"); // Показываем время с одним знаком после запятой
            yield return null;
        }
        if (bonusTimerText != null)
            bonusTimerText.gameObject.SetActive(false);
        currentCloneEffect = null;
        isAnyEffectActive = false;
    }

    public void OnBonusMissed()
    {
        isBonusActive = false;
        currentBonus = null;
    }

    public void StopAllBonuses()
    {
        isAnyEffectActive = false;
        if (bonusTimerText != null)
            bonusTimerText.gameObject.SetActive(false);
        if (_Homing != null) _Homing.SetActive(false);
        if (_DoublePoints != null) _DoublePoints.SetActive(false);
        if (currentHomingEffect != null) StopCoroutine(currentHomingEffect);
        if (currentPointsEffect != null) StopCoroutine(currentPointsEffect);
        if (currentCloneEffect != null) StopCoroutine(currentCloneEffect);
        currentHomingEffect = null;
        currentPointsEffect = null;
        currentCloneEffect = null;

        // Останавливаем вращение мяча при принудительной остановке бонусов
        GameObject activeBall = GameObject.FindGameObjectWithTag("Ball");
        if (activeBall != null)
        {
            // Проверяем, является ли активный мяч мячом с индексом 10
            bool shouldStopRotation = false;
            
            // Находим BallSkinLoader и проверяем, какой мяч активен
            BallSkinLoader ballLoader = FindObjectOfType<BallSkinLoader>();
            if (ballLoader != null && ballLoader.ballPrefabs != null && ballLoader.ballPrefabs.Length > 10)
            {
                // Проверяем, активен ли мяч с индексом 10 (11-й элемент массива)
                if (ballLoader.ballPrefabs[10] != null && ballLoader.ballPrefabs[10].activeInHierarchy)
                {
                    shouldStopRotation = true;
                }
            }
            
            if (shouldStopRotation)
            {
                Rigidbody2D ballRb = activeBall.GetComponent<Rigidbody2D>();
                if (ballRb != null)
                {
                    ballRb.angularVelocity = 0f; // Останавливаем вращение
                }
            }
        }

        // Удаляем только созданные этим скриптом клоны
        foreach (var clone in spawnedClones)
        {
            if (clone != null)
                GameObject.Destroy(clone);
        }
        spawnedClones.Clear();
    }
}
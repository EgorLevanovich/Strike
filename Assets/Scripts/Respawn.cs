using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Respawn : MonoBehaviour
{
    public GameObject[] _ball;
    public GameObject _menuShit;
    public GameObject _playerShit;
    public GameObject _enemyShit;
    public GameObject _gameShit;
    public GameObject _teamShit;
    public GameObject _score;
    public GameObject _pause;
    public Text sessionPointsText;
    public Text sessionPointsText2;
    public KillCountDisplay killCountDisplay;
    public GameObject maps;
    public AudioSource deathMenuMusicSource;
    public GameObject textToHide;
    public GameObject objectToHide;
    private bool gameStarted = false;
    public GameObject[] _all;
    private float startDelay = 0.5f; // Задержка для защиты от ложного срабатывания
    [SerializeField] private Button _respawnButton;
    public GameObject ballPrefab; // Добавьте в инспекторе ссылку на префаб мяча
    private bool respawnAdShown = false; // Флаг для отслеживания показа рекламы


    private void Start()
    {
        StartCoroutine(EnableGameStartAfterDelay());
        
        
    }

    private IEnumerator EnableGameStartAfterDelay()
    {
        yield return new WaitForSeconds(startDelay);
        gameStarted = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!gameStarted) return;
        if (collision.gameObject.CompareTag("Ball"))
        {
            // Этот цикл должен быть закомментирован, чтобы мяч не деактивировался
            // foreach (GameObject obj in _ball)
            // {
            //     if (obj != null)
            //         obj.SetActive(false);
            // }

            foreach (GameObject obj in _all)
            {
                // Важно: эта проверка предотвращает деактивацию _menuShit
                if (obj != null && obj != _menuShit)
                    obj.SetActive(false);
            }

            _score.SetActive(false);
            _pause.SetActive(false);
            Time.timeScale = 0f;

            if (maps != null)
            {
                foreach (var src in maps.GetComponentsInChildren<AudioSource>())
                {
                    if (src != null)
                        src.Stop();
                }
            }
            if (deathMenuMusicSource != null)
                deathMenuMusicSource.Play();

            if (sessionPointsText != null)
            {
                sessionPointsText.text = EnemyPointsGiver.GetTotalKills().ToString();
            }
            if (sessionPointsText2 != null && killCountDisplay != null)
            {
                sessionPointsText2.text = EnemyPointsGiver.GetTotalKills().ToString();
            }
            // var adButton = FindObjectOfType<RewardedAdButton>();
            // if (adButton != null) adButton.ShowRewardButton();
            
            // Важно: активация _menuShit должна быть в самом конце
            _menuShit.SetActive(true);
        }
    }

    void Update()
    {
        if (_menuShit != null && _menuShit.activeInHierarchy)
        {
            if (maps != null)
            {
                foreach (var src in maps.GetComponentsInChildren<AudioSource>())
                {
                    if (src != null && src.isPlaying)
                        src.Stop();
                }
            }
            if (deathMenuMusicSource != null && !deathMenuMusicSource.isPlaying)
                deathMenuMusicSource.Play();

            if (textToHide != null)
                textToHide.SetActive(false);
            if (objectToHide != null)
                objectToHide.SetActive(false);
        }
    }

    private void OnRespawnRequested()
    {
        if (respawnAdShown)
        {
            Debug.Log("Реклама для респавна уже была показана в этой сессии. Перезапустите сцену для повторного показа.");
            // Здесь можно добавить уведомление для игрока, если нужно
            return;
        }
        respawnAdShown = true;
        AdsInitializer.Instance.LoadRewarded();
        AdsInitializer.Instance.ShowRewarded(OnRespawned, OnRespawnFailured);
    }

    private void OnRespawned()
    {
        Debug.Log("OnRespawned - Soft Respawn Activated");

        // 1. Снимаем паузу
        Time.timeScale = 1f;

        // 2. Устанавливаем 1 сердечко через HealthSystem
        var healthSystem = FindObjectOfType<HealthSystem>();
        if (healthSystem != null)
        {
            healthSystem.SetCurrentHP(1);
        }

        // 3. Уничтожаем заспавнившиеся шары (враги), которые опустились ниже 700 по Y
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var enemy in enemies)
        {
            if (enemy != null && enemy.transform.position.y < 700f)
            {
                Destroy(enemy);
            }
        }

        // 4. Возвращаем мяч на (0, 600)
        if (_ball != null && _ball.Length > 0 && _ball[0] != null)
        {
            _ball[0].SetActive(true);

            BallMovement ballMovement = _ball[0].GetComponent<BallMovement>();
            BallBounce ballBounce = _ball[0].GetComponent<BallBounce>(); // Получаем компонент BallBounce
            Rigidbody2D ballRb = _ball[0].GetComponent<Rigidbody2D>();

            // Эти строки должны быть закомментированы
            // if (ballMovement != null)
            // {
            //     ballMovement.enabled = false; // Временно отключаем скрипт движения мяча
            // }
            // if (ballRb != null)
            // {
            //     ballRb.isKinematic = true; // Временно делаем мяч кинематическим, чтобы физика не влияла
            // }

            _ball[0].transform.position = new Vector3(0f, 600f, 0f);
            Debug.Log("Мяч установлен на позицию: " + _ball[0].transform.position);
            
            if (ballRb != null)
            {
                ballRb.velocity = Vector2.zero;
                ballRb.angularVelocity = 0f;
            }

            // Запускаем корутину для установки начальной скорости и активации физики/скриптов
            StartCoroutine(SetBallInitialMotion(ballMovement, ballBounce, ballRb));
        }
        else
        {
            Debug.LogError("Ошибка: Мяч (_ball[0]) не назначен или уничтожен! Не могу респавнить.");
        }

        // 5. Рестартуем спавн
        var spawner = FindObjectOfType<EnemySpawner>();
        if (spawner != null && spawner.gameObject.activeInHierarchy)
        {
            spawner.StopAllCoroutines();
            spawner.StartSpawningOnRespawn();
        }

        // 6. Скрываем меню смерти и возвращаем исходный UI
        if (_menuShit != null)
            _menuShit.SetActive(false);

        if (_score != null)
            _score.SetActive(true);
        if (_pause != null)
            _pause.SetActive(true);
        if (textToHide != null)
            textToHide.SetActive(true);
        if (objectToHide != null)
            objectToHide.SetActive(true);

        // Активируем все объекты в _all array
        foreach (GameObject obj in _all)
        {
            if (obj != null)
                obj.SetActive(true);
        }

        // Останавливаем музыку меню смерти и возобновляем музыку карты
        if (deathMenuMusicSource != null && deathMenuMusicSource.isPlaying)
            deathMenuMusicSource.Stop();

        if (maps != null)
        {
            foreach (var src in maps.GetComponentsInChildren<AudioSource>())
            {
                if (src != null && !src.isPlaying)
                    src.Play();
            }
        }

        // Сбрасываем флаг респавна, чтобы GameManager и HealthSystem знали, что следующий старт не респавн
        PlayerPrefs.SetInt("RespawnActive", 0);
        PlayerPrefs.Save();
    }

    private void OnRespawnFailured()
    {
        Debug.Log("Respawn failed: Ad was not watched or failed to load.");
        // Здесь можно добавить дополнительную логику, например, показать сообщение игроку
    }

    // Корутина для установки начальной скорости мяча и включения физики/скриптов после небольшой задержки
    private IEnumerator SetBallInitialMotion(BallMovement ballMovement, BallBounce ballBounce, Rigidbody2D ballRb)
    {
        yield return new WaitForFixedUpdate(); // Ждем один физический апдейт

        if (ballRb != null)
        {
            ballRb.isKinematic = false; // Возвращаем мяч в физический режим
        }

        // Устанавливаем начальную скорость падения, используя скорость из BallBounce
        if (ballRb != null && ballBounce != null)
        {
            ballRb.velocity = new Vector2(0f, -ballBounce._speed);
        }

        // Включаем BallMovement обратно, чтобы он продолжал управлять падением
        if (ballMovement != null)
        {
            ballMovement.enabled = true;
        }
    }

    
}

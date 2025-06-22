using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallBounce : MonoBehaviour
{

    private Rigidbody2D _rigidbody2D;
    public float _bounceForce = 10;
    public float _speed = 10f;
    public float _sideBounce;
    public AudioSource bounceAudio;
    public GameObject _menuShit;
    public GameObject _score;
    public GameObject _pause;
    public GameObject maps;
    public AudioSource deathMenuMusicSource;
    public Text sessionPointsText;
    public Text sessionPointsText2;
    public KillCountDisplay killCountDisplay;
    public GameObject[] _all;
    public GameObject[] _ball;
    public GameObject textToHide;
    public GameObject objectToHide;
    public AudioSource BallBouncePlatformAudio;
    public AudioSource BonusAudio;
    [SerializeField] private Button _respawnButton;
    public Text countdownText; // UI текст для отображения таймера
    private bool respawnAdShown = false; // Флаг для отслеживания показа рекламы

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _rigidbody2D.velocity = new Vector2(0f,-_speed);
        _respawnButton.onClick.AddListener(OnRespawnRequested);
        SetRespawnButtonState();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Respawn"))
        {
            var healthSystem = FindObjectOfType<HealthSystem>();
            if (healthSystem != null)
            {
                healthSystem.Die();
            }
            return;
        }

        if (collision.gameObject.CompareTag("Ground"))
        {
            if (BallBouncePlatformAudio != null) BallBouncePlatformAudio.Play();
            float ballX = transform.position.x;
            float platformX = collision.transform.position.x;
            float offset = ballX - platformX; // >0 — справа, <0 — слева
            float directionX = Mathf.Sign(offset);
            float xForce = directionX * Mathf.Min(Mathf.Abs(offset) * 2f, _bounceForce * 0.7f); // сила по X ограничена
            _rigidbody2D.velocity = new Vector2(xForce, _bounceForce);
        }
        else if (collision.gameObject.CompareTag("Walls"))
        {
            Vector2 normal = collision.contacts[0].normal;
            _rigidbody2D.velocity = Vector2.Reflect(_rigidbody2D.velocity, normal);
            _rigidbody2D.velocity += new Vector2(normal.x * _sideBounce, 0f);
            if (bounceAudio != null) bounceAudio.Play();
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            Vector2 normal = collision.contacts[0].normal;
            _rigidbody2D.velocity = Vector2.Reflect(_rigidbody2D.velocity, normal);
            _rigidbody2D.velocity += new Vector2(normal.x * (_sideBounce * 0.8f), (_sideBounce * 0.8f) * Mathf.Sign(normal.y));
        }
        else if (collision.gameObject.CompareTag("UpperWalls"))
        {
              Vector2 normal = collision.contacts[0].normal;
            _rigidbody2D.velocity = Vector2.Reflect(_rigidbody2D.velocity, normal);
            _rigidbody2D.velocity += new Vector2(normal.x * (_sideBounce * 0.8f), (_sideBounce * 0.8f) * Mathf.Sign(normal.y));
            if (bounceAudio != null) bounceAudio.Play();
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
          
            Vector2 normal = collision.contacts[0].normal;
            _rigidbody2D.velocity = Vector2.Reflect(_rigidbody2D.velocity, normal);
            _rigidbody2D.velocity += new Vector2(normal.x * (_sideBounce * 0.8f), (_sideBounce * 0.8f) * Mathf.Sign(normal.y));
           
        }
        else if (collision.gameObject.CompareTag("Ball"))
        {
            // Отскок двух мячей друг от друга
            Rigidbody2D otherRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (otherRb != null)
            {
                // Вектор между центрами мячей
                Vector2 direction = (transform.position - collision.transform.position).normalized;
                // Сила отскока
                float bounceStrength = _bounceForce * 0.175f;
                // Каждый мяч получает импульс в противоположную сторону
                _rigidbody2D.velocity = direction * bounceStrength;
                otherRb.velocity = -direction * bounceStrength;
            }
        }
    }

    private IEnumerator ForceVelocity(Vector2 v)
    {
        yield return new WaitForFixedUpdate();
        _rigidbody2D.velocity = v;
        _rigidbody2D.angularVelocity = 0f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_rigidbody2D != null && this.enabled)
        {
            Debug.Log($"BallBounce FixedUpdate: Position = {transform.position}, Velocity = {_rigidbody2D.velocity}");
        }
    }

    private void OnRespawnRequested()
    {
        if (respawnAdShown)
        {
            Debug.Log("Реклама для респавна уже была показана в этой сессии. Перезапустите сцену для повторного показа.");
            return;
        }
        respawnAdShown = true;
        SetRespawnButtonState();
        AdsInitializer.Instance.LoadRewarded();
        AdsInitializer.Instance.ShowRewarded(OnRespawned, OnRespawnFailured);
    }

    private void OnRespawnFailured()
    {
        Debug.Log("Rewarded ad failed to show or was skipped.");
        SetRespawnButtonState();
        // Здесь вы можете добавить логику, что делать, если реклама не показана или пропущена.
        // Например, показать сообщение пользователю, предложить другую опцию или просто закрыть меню.
        // Сейчас просто закроем меню и вернем время.
        Time.timeScale = 1f;
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
        foreach (GameObject obj in _all)
        {
            if (obj != null)
                obj.SetActive(true);
        }
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
    }

    private void OnRespawned()
    {
        Debug.Log("OnRespawned - Soft Respawn Activated");
        SetRespawnButtonState();
        // 1. Ставим игру на паузу и запускаем отсчет
        Time.timeScale = 0f;
        if (countdownText != null)
            countdownText.gameObject.SetActive(true);
        StartCoroutine(CountdownAndResumeGame());
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
            if (enemy != null && enemy.transform.position.y < 1650f)
            {
                Destroy(enemy);
            }
        }

        // 4. Возвращаем активный мяч с тегом Ball на (0, 400, 0)
        GameObject activeBall = GameObject.FindGameObjectWithTag("Ball");
        if (activeBall != null)
        {
            activeBall.SetActive(true);
            BallMovement ballMovement = activeBall.GetComponent<BallMovement>();
            BallBounce ballBounce = activeBall.GetComponent<BallBounce>();
            Rigidbody2D ballRb = activeBall.GetComponent<Rigidbody2D>();
            if (ballMovement != null) ballMovement.enabled = false;
            if (ballRb != null)
            {
                ballRb.isKinematic = true;
                ballRb.velocity = Vector2.zero;
                ballRb.angularVelocity = 0f;
            }
            RectTransform rect = activeBall.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchoredPosition = Vector2.zero;
                rect.localPosition = Vector3.zero;
                Debug.Log("RectTransform мяча установлен в центр родителя (anchoredPosition и localPosition = 0)!");
            }
            else
            {
                activeBall.transform.position = activeBall.transform.parent != null ? activeBall.transform.parent.position : new Vector3(0f, 400f, 0f);
            }
            StartCoroutine(SetBallInitialMotion(ballMovement, ballBounce, ballRb));
            if (ballRb != null)
            {
                StartCoroutine(EnablePhysicsAndMovementAfterDelay(ballRb, ballMovement));
            }
        }
        else
        {
            Debug.LogError("Ошибка: Не найден активный GameObject с тегом Ball!");
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

    private IEnumerator SetBallInitialMotion(BallMovement ballMovement, BallBounce ballBounce, Rigidbody2D ballRb)
    {
        yield return new WaitForFixedUpdate(); // Wait for the next fixed physics update

        if (ballMovement != null)
        {
            ballMovement.enabled = true; // Re-enable ball movement script
        }

        if (ballRb != null)
        {
            ballRb.isKinematic = false; // Re-enable physics
            ballRb.velocity = new Vector2(0f, -_speed); // Set initial downward velocity
        }
    }

    private void SetRespawnButtonState()
    {
        if (_respawnButton != null)
        {
            if (respawnAdShown)
            {
                _respawnButton.interactable = false;
                var colors = _respawnButton.colors;
                colors.normalColor = new Color(colors.normalColor.r, colors.normalColor.g, colors.normalColor.b, 0.5f);
                colors.disabledColor = new Color(colors.disabledColor.r, colors.disabledColor.g, colors.disabledColor.b, 0.5f);
                _respawnButton.colors = colors;
            }
            else
            {
                _respawnButton.interactable = true;
                var colors = _respawnButton.colors;
                colors.normalColor = new Color(colors.normalColor.r, colors.normalColor.g, colors.normalColor.b, 1f);
                colors.disabledColor = new Color(colors.disabledColor.r, colors.disabledColor.g, colors.disabledColor.b, 1f);
                _respawnButton.colors = colors;
            }
        }
    }

    private IEnumerator EnablePhysicsAndMovementAfterDelay(Rigidbody2D ballRb, BallMovement ballMovement)
    {
        yield return new WaitForFixedUpdate();
        if (ballRb != null)
            ballRb.isKinematic = false;
        if (ballMovement != null)
            ballMovement.enabled = true;
    }

    private IEnumerator CountdownAndResumeGame()
    {
        float timer = 3f;
        while (timer > 0)
        {
            if (countdownText != null)
                countdownText.text = Mathf.Ceil(timer).ToString();
            yield return new WaitForSecondsRealtime(1f);
            timer -= 1f;
        }
        if (countdownText != null)
        {
            countdownText.text = "";
            countdownText.gameObject.SetActive(false);
        }
        Time.timeScale = 1f;
        // Здесь можно вернуть остальной код респавна, если он был перенесен
    }
}

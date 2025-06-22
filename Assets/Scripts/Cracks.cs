using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement; // Добавляем для PlayerPrefs
using System.Linq;

public class HealthSystem : MonoBehaviour
{
    [Header("Настройки HP")]
    [SerializeField] private GameObject[] hearts; // Массив сердечек
    [SerializeField] private int maxHP = 3;
    [SerializeField] private string damagingTag = "Border";
    public GameObject _menu; // Меню смерти
    [SerializeField] private Button _respawnButton;
    private bool respawnAdShown = false;
    private bool isAdShowing = false; // <--- добавлен флаг ожидания рекламы
    [SerializeField] private Button _interstitialRespawnButton; // Кнопка для interstitial респавна
    private bool interstitialAdShown = false;
    private bool isInterstitialAdShowing = false;

    // --- Новые поля для общей логики респавна (из BallBounce) ---
    public GameObject _score; // Объект UI счета
    public GameObject _pause; // Объект UI паузы
    public GameObject textToHide;
    public GameObject objectToHide;
    public GameObject[] _all; // Массив всех игровых объектов для активации/деактивации
    public GameObject maps; // Объект с аудиоисточниками карты
    public Text countdownText; // UI текст для отображения таймера
    public Text sessionPointsText; // Текст для очков сессии
    public Text sessionPointsText2; // Дополнительный текст для очков сессии
    public KillCountDisplay killCountDisplay; // Объект для отображения количества убийств
    public GameObject[] _ball; // Массив мячей (для респавна активного)
    public AudioSource deathMenuMusicSource; // Добавлен для музыки меню смерти
    public GameObject Defeat; // Объект Defeat для управления его музыкой
    public AudioSource mainMapMusic;

    private int currentHP;
    private Vector3 _respawnPosition = new Vector3(0f, 25f, 0f); // Позиция для респавна мяча

    private void Start()
    {
        currentHP = maxHP;
        UpdateHeartsUI();
        if (_respawnButton != null)
            _respawnButton.onClick.AddListener(OnRespawnRequested);
        if (_interstitialRespawnButton != null)
            _interstitialRespawnButton.onClick.AddListener(OnInterstitialRespawnRequested);
        SetRespawnButtonState();
        AdsInitializer.Instance.LoadRewarded(); // Загружаем рекламу заранее
        AdsInitializer.Instance.LoadInterstitial(); // Загружаем interstitial заранее
        // Подписка на событие загрузки рекламы
        if (AdsInitializer.Instance != null)
        {
            AdsInitializer.Instance.OnRewardedLoaded += SetRespawnButtonState;
            AdsInitializer.Instance.OnInterstitialLoaded += SetRespawnButtonState;
        }
    }

    public void SetCurrentHP(int newHP)
    {
        currentHP = Mathf.Clamp(newHP, 0, maxHP);
        UpdateHeartsUI();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(damagingTag))
        {
            TakeDamage();
        }
    }

    private void TakeDamage()
    {
        if (currentHP > 0)
        {
            currentHP--;
            UpdateHeartsUI();

            if (currentHP <= 0)
            {
                Die();
            }
        }
    }

    private void UpdateHeartsUI()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].SetActive(i < currentHP);
        }
    }

    public void Die()
    {
        // Скрываем таймер бонуса всегда при смерти
        if (objectToHide != null) objectToHide.SetActive(false);
        // Прекращаем действие бонуса
        var bonusSystem = FindObjectOfType<BonusSystem2D>();
        if (bonusSystem != null) bonusSystem.StopAllBonuses();
        
        // Останавливаем время
        Time.timeScale = 0f;
        
        // Показываем меню смерти
        if (_menu != null)
        {
            _menu.SetActive(true);
            objectToHide.SetActive(false);
        }
        SetRespawnButtonState();
         SetCurrentHP(1);
        // Скрываем основные игровые UI элементы
        if (_score != null) _score.SetActive(false);
        if (_pause != null) _pause.SetActive(false);
        if (textToHide != null) textToHide.SetActive(false);
        if (objectToHide != null) objectToHide.SetActive(false);

        // Обновление текстов очков перед показом меню смерти
        if (sessionPointsText != null)
        {
            sessionPointsText.text = EnemyPointsGiver.GetTotalKills().ToString();
        }
        if (sessionPointsText2 != null && killCountDisplay != null)
        {
            sessionPointsText2.text = EnemyPointsGiver.GetTotalKills().ToString();
        }

        // 1. Удаляем всех врагов ниже экрана
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        var bonus = GameObject.FindGameObjectWithTag("Bonus");
        foreach (var enemy in enemies)
        {
            if (enemy != null && enemy.transform.position.y < 1650f )
            {
                Destroy(enemy);
            }

            if (bonus != null && bonus.transform.position.y < 1650f )
            {
                Destroy(bonus);
            }
        }
        // 2. Активируем и перемещаем мяч
        GameObject activeBall = GameObject.FindGameObjectWithTag("Ball");
        if (activeBall != null)
        {
            activeBall.SetActive(true);
            if (activeBall.transform.parent != null)
                activeBall.transform.position = activeBall.transform.parent.position;
            else
                activeBall.transform.position = new Vector3(0f, 0f, 0f);
            var ballRb = activeBall.GetComponent<Rigidbody2D>();
            if (ballRb != null)
            {
                ballRb.velocity = Vector2.zero;
                ballRb.angularVelocity = 0f;
                ballRb.isKinematic = true;
            }
            var ballMovement = activeBall.GetComponent<BallMovement>();
            if (ballMovement != null) ballMovement.enabled = false;
        }
        // Перемещаем активную платформу на позицию родителя (как мяч) сразу (дублируем для надёжности)
        var platformMovement = FindObjectOfType<PlatformMovement>();
        if (platformMovement != null && platformMovement._players != null && platformMovement._players.Length > 0)
        {
            for (int i = 0; i < platformMovement._players.Length; i++)
            {
                var plat = platformMovement._players[i];
                if (plat != null && plat.gameObject.activeSelf)
                {
                    plat.gameObject.SetActive(true);
                    if (plat.transform.parent != null)
                        plat.transform.position = plat.transform.parent.position;
                    else
                        plat.transform.position = new Vector3(0f, 0f, plat.transform.position.z);
                    plat.velocity = Vector2.zero;
                    plat.angularVelocity = 0f;
                }
            }
        }
        // Перемещаем активную платформу через 1 кадр (для билда)
        StartCoroutine(MoveActivePlatformToParentNextFrame());
       
       if (objectToHide != null)
        {
            if (bonusSystem != null && bonusSystem.isAnyEffectActive)
                objectToHide.SetActive(true);
            else
                objectToHide.SetActive(false);
        }

        // Ставим на паузу музыку карты
        if (maps != null)
        {
            foreach (Transform map in maps.transform)
            {
                if (map.gameObject.activeInHierarchy)
                {
                    var music = map.GetComponentInChildren<AudioSource>();
                    if (music != null && music.isPlaying)
                        music.Pause();
                }
            }
        }
        // Ставим на паузу всю игровую музыку через musicPause
        var allMusicPauseComponents = GameObject.FindObjectsOfType<musicPause>(true); // true — ищет и в неактивных
        foreach (var music in allMusicPauseComponents)
        {
            var src = music.GetComponent<AudioSource>();
            if (src != null && src.isPlaying)
                src.Pause();
        }
        // Включаем музыку меню смерти
        if (deathMenuMusicSource != null)
        {
            deathMenuMusicSource.Play();
        }

        // Включаем AudioSource на Defeat при появлении меню смерти
        if (Defeat != null)
        {
            var audio = Defeat.GetComponent<AudioSource>();
            if (audio != null)
            {
                audio.Play();
                Debug.Log("[HealthSystem] Включена музыка на объекте Defeat");
            }
        }

        if (mainMapMusic != null && !mainMapMusic.isPlaying)
        {
            if (mainMapMusic.time > 0f)
                mainMapMusic.UnPause();
            else
                mainMapMusic.Play();
        }
    }

    private IEnumerator MoveActivePlatformToParentNextFrame()
    {
        yield return null; // Ждём 1 кадр для гарантии

        var platformMovement = FindObjectOfType<PlatformMovement>();
        if (platformMovement != null && platformMovement._players != null && platformMovement._players.Length > 0)
        {
            for (int i = 0; i < platformMovement._players.Length; i++)
            {
                var plat = platformMovement._players[i];
                if (plat != null && plat.gameObject.activeSelf)
                {
                    plat.gameObject.SetActive(true); // На всякий случай активируем
                    if (plat.transform.parent != null)
                    {
                        plat.transform.position = plat.transform.parent.position;
                        Debug.Log($"[Cracks] Platform {plat.name} moved to parent {plat.transform.parent.name} position: {plat.transform.position}");
                    }
                    else
                    {
                        plat.transform.position = new Vector3(0f, 0f, plat.transform.position.z);
                        Debug.Log($"[Cracks] Platform {plat.name} moved to (0,0,z): {plat.transform.position}");
                    }
                    plat.velocity = Vector2.zero;
                    plat.angularVelocity = 0f;
                }
            }
        }
    }

    private void OnRespawnRequested()
    {
        if (respawnAdShown || isAdShowing)
        {
            Debug.Log("Реклама для респавна уже была показана или уже показывается.");
            return;
        }
        if (!AdsInitializer.Instance.IsRewarded())
        {
            Debug.Log("Реклама еще не загружена!");
            return;
        }
        respawnAdShown = true;
        isAdShowing = true;
        SetRespawnButtonState();
        // Скрываем меню смерти и останавливаем музыку меню смерти сразу при открытии рекламы
        if (_menu != null)
            _menu.SetActive(false);
        if (deathMenuMusicSource != null && deathMenuMusicSource.isPlaying)
            deathMenuMusicSource.Stop();
        StartCoroutine(ShowRewardedWithDelay());
    }

    private IEnumerator ShowRewardedWithDelay()
    {
        AdsInitializer.Instance.ShowRewarded(OnAdWatched, OnRespawnFailured);
        yield break;
    }

    private void OnAdWatched()
    {
        isAdShowing = false;
        StartCoroutine(RespawnWithDelay());
    }

    private void OnRespawned()
    {
        Time.timeScale = 1f;
        if (_menu != null) _menu.SetActive(false);
        if (_score != null) _score.SetActive(true);
        if (_pause != null) _pause.SetActive(true);
        // Таймер не появляется, пока не начнётся новый бонус
        if (objectToHide != null) objectToHide.SetActive(false);
        foreach (GameObject obj in _all)
        {
            if (obj != null) obj.SetActive(true);
        }
        // Управляем таймером бонуса (objectToHide)
        var bonusSystem = FindObjectOfType<BonusSystem2D>();
        if (objectToHide != null)
        {
            if (bonusSystem != null && bonusSystem.isAnyEffectActive)
                objectToHide.SetActive(true);
            else
                objectToHide.SetActive(false);
        }
        // Останавливаем музыку меню смерти
        if (deathMenuMusicSource != null && deathMenuMusicSource.isPlaying)
        {
            deathMenuMusicSource.Stop();
        }
        // Снимаем с паузы музыку карты
        if (maps != null)
        {
            foreach (var src in maps.GetComponentsInChildren<AudioSource>(true))
            {
                if (src != null && !src.isPlaying)
                {
                    if (src.time > 0f)
                        src.UnPause();
                    else
                        src.Play();
                }
            }
        }
        // Останавливаем музыку на объекте Defeat
        if (Defeat != null)
        {
            var defeatAudio = Defeat.GetComponent<AudioSource>();
            if (defeatAudio != null && defeatAudio.isPlaying)
            {
                defeatAudio.Stop();
                
            }
        }
        // Универсальное возобновление музыки карты
        
        
        // Включаем физику и движение мяча
        GameObject activeBall = GameObject.FindGameObjectWithTag("Ball");
        if (activeBall != null)
        {
            var ballRb = activeBall.GetComponent<Rigidbody2D>();
            var ballMovement = activeBall.GetComponent<BallMovement>();
            if (ballMovement != null) ballMovement.enabled = true;
            if (ballRb != null)
            {
                ballRb.isKinematic = false;
                ballRb.velocity = new Vector2(0f, -10f); // Можно скорректировать скорость
            }
        }
        // Делаем таймер видимым
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(true);
            countdownText.text = "3";
        }
        StartCoroutine(CountdownAndResumeGame());
        // Снимаем паузу со всей игровой музыки через musicPause
        var allMusicPauseComponents = GameObject.FindObjectsOfType<musicPause>(true);
        foreach (var music in allMusicPauseComponents)
        {
            var src = music.GetComponent<AudioSource>();
            if (src != null && !src.isPlaying)
            {
                if (src.time > 0f)
                    src.UnPause();
                else
                    src.Play();
            }
        }
    }

    private void OnRespawnFailured()
    {
        isAdShowing = false;
        // Останавливаем все AudioSource, на которых висит скрипт musicDeathMenuPause
        var deathMenuSources = GameObject.FindObjectsOfType<musicDeathMenuPause>();
        foreach (var music in deathMenuSources)
        {
            music.StopMusic();
        }
        SetRespawnButtonState();
        Debug.Log("Respawn failed: Ad was not watched or failed to load. Closing menu and resuming game.");
        AdsInitializer.Instance.LoadRewarded(); // Загружаем рекламу для следующего раза
        // В случае неудачи с рекламой: закрыть меню смерти и продолжить игру
        Time.timeScale = 1f;
        if (_menu != null) _menu.SetActive(false);
        if (_score != null) _score.SetActive(true);
        if (_pause != null) _pause.SetActive(true);
        if (textToHide != null) textToHide.SetActive(true);
        if (objectToHide != null) objectToHide.SetActive(true);
        foreach (GameObject obj in _all)
        {
            if (obj != null)
                obj.SetActive(true);
        }
        if (maps != null)
        {
            foreach (var src in maps.GetComponentsInChildren<AudioSource>(true))
            {
                if (src != null && !src.isPlaying)
                    src.Play();
            }
        }

        // Возобновляем все AudioSource, на которых висит скрипт musicPause
        var musicSources = GameObject.FindObjectsOfType<MonoBehaviour>().OfType<musicPause>();
        foreach (var music in musicSources)
        {
            var audio = music.GetComponent<AudioSource>();
            if (audio != null)
                audio.UnPause();
        }

        // Останавливаем музыку меню смерти при закрытии меню смерти (при неудаче рекламы)
        if (deathMenuMusicSource != null && deathMenuMusicSource.isPlaying)
        {
            deathMenuMusicSource.Stop();
            Debug.Log("[HealthSystem] deathMenuMusicSource остановлен при закрытии меню смерти (fail).");
        }
    }

    private void OnInterstitialRespawnRequested()
    {
        if (interstitialAdShown || isInterstitialAdShowing)
        {
            Debug.Log("Interstitial уже была показана или показывается.");
            return;
        }
        if (!AdsInitializer.Instance.IsInterstitialReady())
        {
            Debug.Log("Interstitial еще не загружена!");
            return;
        }
        interstitialAdShown = true;
        isInterstitialAdShowing = true;
        SetRespawnButtonState();
        // Устанавливаем позицию для респавна перед показом рекламы
        _respawnPosition = new Vector3(0f, 25f, 0f);
        // Показываем рекламу сразу, без задержки
        AdsInitializer.Instance.ShowInterstitial(OnInterstitialAdWatched, OnInterstitialRespawnFailured);
    }

    private void OnInterstitialAdWatched()
    {
        isInterstitialAdShowing = false;
        StartCoroutine(RespawnWithDelay());
    }

    private void OnInterstitialRespawnFailured()
    {
        isInterstitialAdShowing = false;
        // Можно добавить отдельную логику для неудачного interstitial
        Debug.Log("Interstitial не была просмотрена до конца или не загрузилась.");
        SetRespawnButtonState();
    }

    private void SetRespawnButtonState()
    {
        if (_respawnButton != null)
        {
            bool adReady = AdsInitializer.Instance != null && AdsInitializer.Instance.IsRewarded();
            _respawnButton.interactable = !respawnAdShown && adReady && !isAdShowing;
            var colors = _respawnButton.colors;
            colors.normalColor = new Color(colors.normalColor.r, colors.normalColor.g, colors.normalColor.b, (!respawnAdShown && adReady && !isAdShowing) ? 1f : 0.5f);
            colors.disabledColor = new Color(colors.disabledColor.r, colors.disabledColor.g, colors.disabledColor.b, (!respawnAdShown && adReady && !isAdShowing) ? 1f : 0.5f);
            _respawnButton.colors = colors;
        }
        if (_interstitialRespawnButton != null)
        {
            bool adReady = AdsInitializer.Instance != null && AdsInitializer.Instance.IsInterstitialReady();
            _interstitialRespawnButton.interactable = !interstitialAdShown && adReady && !isInterstitialAdShowing;
            var colors = _interstitialRespawnButton.colors;
            colors.normalColor = new Color(colors.normalColor.r, colors.normalColor.g, colors.normalColor.b, (!interstitialAdShown && adReady && !isInterstitialAdShowing) ? 1f : 0.5f);
            colors.disabledColor = new Color(colors.disabledColor.r, colors.disabledColor.g, colors.disabledColor.b, (!interstitialAdShown && adReady && !isInterstitialAdShowing) ? 1f : 0.5f);
            _interstitialRespawnButton.colors = colors;
        }
    }

    // --- Корутины, перемещенные из BallBounce ---

    private IEnumerator SetBallInitialMotionAndPhysicsEnable(BallMovement ballMovement, BallBounce ballBounce, Rigidbody2D ballRb)
    {
        yield return new WaitForFixedUpdate();
        if (ballMovement != null) ballMovement.enabled = true;
        if (ballRb != null)
        {
            ballRb.isKinematic = false;
            if (ballBounce != null)
                ballRb.velocity = new Vector2(0f, -ballBounce._speed);
            else
                ballRb.velocity = new Vector2(0f, -10f);
        }
    }

    private IEnumerator CountdownAndResumeGame()
    {
        Time.timeScale = 0f; // Форсируем паузу ещё раз
        float timer = 3f;
        if (countdownText != null)
            countdownText.gameObject.SetActive(true);

        // Делаем кнопку паузы неактивной для нажатия на время отсчёта
        if (_pause != null)
        {
            var pauseButton = _pause.GetComponent<Button>();
            if (pauseButton != null)
                pauseButton.interactable = false;
        }

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

        // Включаем кнопку паузы обратно
        if (_pause != null)
        {
            var pauseButton = _pause.GetComponent<Button>();
            if (pauseButton != null)
                pauseButton.interactable = true;
        }

        Time.timeScale = 1f;
    }

    private IEnumerator RespawnWithDelay()
    {
        yield return new WaitForSecondsRealtime(0.3f); // Даем сцене "проснуться" после рекламы
        OnRespawned();
    }

    private void OnDestroy()
    {
        if (AdsInitializer.Instance != null)
        {
            AdsInitializer.Instance.OnRewardedLoaded -= SetRespawnButtonState;
            AdsInitializer.Instance.OnInterstitialLoaded -= SetRespawnButtonState;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DeathCollider : MonoBehaviour
{
    // Public variables will be moved here from BallBounce.cs
    public GameObject _menuShit;
    public GameObject _score;
    public GameObject _pause;
    public GameObject maps;
    public AudioSource deathMenuMusicSource;
    public Text sessionPointsText;
    public Text sessionPointsText2;
    public KillCountDisplay killCountDisplay;
    public GameObject[] _all;
    public GameObject _ball;
    public GameObject textToHide;
    public GameObject objectToHide;
    [SerializeField] private Button _respawnButton;
    public float _speed = 10f; // Needed for SetBallInitialMotion

    // Methods will be moved here from BallBounce.cs
    private void Start()
    {
        if (_respawnButton != null)
        {
            _respawnButton.onClick.AddListener(OnRespawnRequested);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) // Assuming DeathCollider uses OnTriggerEnter2D for collision
    {
        if(collision.gameObject.CompareTag("Respawn"))
        {
            Time.timeScale = 0; 
             _menuShit.SetActive(true);
             foreach (GameObject obj in _all)
            {
                if (obj != null && obj != _menuShit)
                    obj.SetActive(false);
            }

            _score.SetActive(false);
            _pause.SetActive(false);
           
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
        }
    }

    private void OnRespawnRequested()
    {
        AdsInitializer.Instance.LoadRewarded();
        AdsInitializer.Instance.ShowRewarded(OnRespawned, OnRespawnFailured);
    }

    private void OnRespawnFailured()
    {
        Debug.Log("Rewarded ad failed to show or was skipped.");
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

        Time.timeScale = 1f;

        var healthSystem = FindObjectOfType<HealthSystem>();
        if (healthSystem != null)
        {
            healthSystem.SetCurrentHP(1);
        }

        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var enemy in enemies)
        {
            if (enemy != null && enemy.transform.position.y < 700f)
            {
                Destroy(enemy);
            }
        }

        if (_ball != null)
        {
            _ball.SetActive(true);

            BallMovement ballMovement = _ball.GetComponent<BallMovement>();
            BallBounce ballBounce = _ball.GetComponent<BallBounce>(); 
            Rigidbody2D ballRb = _ball.GetComponent<Rigidbody2D>();

            _ball.transform.position = new Vector3(0f, 600f, 0f);
            Debug.Log("Мяч установлен на позицию: " + _ball.transform.position);
            
            if (ballRb != null)
            {
                ballRb.velocity = Vector2.zero;
                ballRb.angularVelocity = 0f;
            }

            StartCoroutine(SetBallInitialMotion(ballMovement, ballBounce, ballRb));
        }
        else
        {
            Debug.LogError("Ошибка: Мяч (_ball) не назначен или уничтожен! Не могу респавнить.");
        }

        var spawner = FindObjectOfType<EnemySpawner>();
        if (spawner != null && spawner.gameObject.activeInHierarchy)
        {
            spawner.StopAllCoroutines();
            spawner.StartSpawningOnRespawn();
        }

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

        PlayerPrefs.SetInt("RespawnActive", 0);
        PlayerPrefs.Save();
    }

    private IEnumerator SetBallInitialMotion(BallMovement ballMovement, BallBounce ballBounce, Rigidbody2D ballRb)
    {
        yield return new WaitForFixedUpdate();

        if (ballMovement != null)
        {
            ballMovement.enabled = true;
        }

        if (ballRb != null)
        {
            ballRb.isKinematic = false;
            ballRb.velocity = new Vector2(0f, -_speed);
        }
    }
} 
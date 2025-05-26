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
   
    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (tag == "Respawn")
        {
            foreach (GameObject obj in _ball)
            {
                if (obj != null)
                    obj.SetActive(false);
            }
            _menuShit.SetActive(true);
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
}

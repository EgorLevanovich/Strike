using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loader : MonoBehaviour
{
   
    [SerializeField] private string _targetSceneName = "MainMenu";
    [SerializeField] private float _minLoadDuration = 3f; // Минимальное время загрузки (для плавности)
    [SerializeField] private Slider _progressBar; // Опциональный прогресс бар

    private AsyncOperation _loadingOperation;
    private float _loadProgress;
    private bool _isReadyToSwitch;

    private void Start()
    {
        StartCoroutine(LoadSceneAsync());
    }

    private IEnumerator LoadSceneAsync()
    {
        // Начинаем асинхронную загрузку
        _loadingOperation = SceneManager.LoadSceneAsync(_targetSceneName);
        _loadingOperation.allowSceneActivation = false; // Запрещаем автоматический переход

        float elapsedTime = 0f;

        while (!_loadingOperation.isDone)
        {
            elapsedTime += Time.deltaTime;

            // Имитируем прогресс (0.9 - максимум для allowSceneActivation=false)
            _loadProgress = Mathf.Clamp01(elapsedTime / _minLoadDuration * 0.9f);

            if (_progressBar != null)
                _progressBar.value = _loadProgress;

            // Когда загрузка действительно завершена и прошло минимальное время
            if (_loadingOperation.progress >= 0.9f && elapsedTime >= _minLoadDuration)
            {
                _isReadyToSwitch = true;
                OnLoadComplete(); // Можно активировать автоматически или по кнопке
                yield break;
            }

            yield return null;
        }
    }

    // Вызывается при клике кнопки или автоматически
    public void OnLoadComplete()
    {
        if (_isReadyToSwitch && _loadingOperation != null)
        {
            _loadingOperation.allowSceneActivation = true;
        }
    }

    // Для отладки в инспекторе
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(_targetSceneName))
        {
            Debug.LogWarning("Укажите имя сцены для загрузки!");
        }
    }
}

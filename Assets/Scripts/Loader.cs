using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loader : MonoBehaviour
{
   
    [SerializeField] private string _targetSceneName = "MainMenu";
    [SerializeField] private float _minLoadDuration = 3f; // ����������� ����� �������� (��� ���������)
    [SerializeField] private Slider _progressBar; // ������������ �������� ���

    private AsyncOperation _loadingOperation;
    private float _loadProgress;
    private bool _isReadyToSwitch;

    private void Start()
    {
        StartCoroutine(LoadSceneAsync());
    }

    private IEnumerator LoadSceneAsync()
    {
        // �������� ����������� ��������
        _loadingOperation = SceneManager.LoadSceneAsync(_targetSceneName);
        _loadingOperation.allowSceneActivation = false; // ��������� �������������� �������

        float elapsedTime = 0f;

        while (!_loadingOperation.isDone)
        {
            elapsedTime += Time.deltaTime;

            // ��������� �������� (0.9 - �������� ��� allowSceneActivation=false)
            _loadProgress = Mathf.Clamp01(elapsedTime / _minLoadDuration * 0.9f);

            if (_progressBar != null)
                _progressBar.value = _loadProgress;

            // ����� �������� ������������� ��������� � ������ ����������� �����
            if (_loadingOperation.progress >= 0.9f && elapsedTime >= _minLoadDuration)
            {
                _isReadyToSwitch = true;
                OnLoadComplete(); // ����� ������������ ������������� ��� �� ������
                yield break;
            }

            yield return null;
        }
    }

    // ���������� ��� ����� ������ ��� �������������
    public void OnLoadComplete()
    {
        if (_isReadyToSwitch && _loadingOperation != null)
        {
            _loadingOperation.allowSceneActivation = true;
        }
    }

    // ��� ������� � ����������
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(_targetSceneName))
        {
            // Debug.LogWarning("   ��� ��������!");
        }
    }
}

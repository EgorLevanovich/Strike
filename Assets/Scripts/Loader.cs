using Assets.Scripts.Gameplay;
using System.Collections;
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

    private string _path;
    private GameplayContainer _container;

    private void Start()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
        if (AdsInitializer.Instance != null)
        {
            AdsInitializer.Instance.LoadRewarded();
            AdsInitializer.Instance.LoadInterstitial();
        }

        _path = System.IO.Path.Combine(Application.persistentDataPath, "gameplay_container");
        var analytics = new Analytics();
        _container = LoadContainer();
        Debug.Log($"{nameof(LoadContainer)} skin_name={_container.PlatformName}, {_container.BallName}, {_container.LevelName}");
        _container.Changed += OnChanged;
 
        StartCoroutine(LoadSceneAsync());

        void OnChanged()
        {
            SaveContainer();
        }
    }

    private GameplayContainer LoadContainer()
    {
        if(System.IO.File.Exists(_path))
        {
            var file = System.IO.File.ReadAllText(_path);
            return JsonUtility.FromJson<GameplayContainer>(file);
        }

        return new GameplayContainer();
    }

    private void SaveContainer()
    {
        var file = JsonUtility.ToJson(_container);
        System.IO.File.WriteAllText(_path, file);
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

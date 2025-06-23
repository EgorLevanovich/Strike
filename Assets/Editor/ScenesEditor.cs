using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class ScenesEditor : EditorWindow
{
    private PopupField<string> _sceneDropdown;
    
    [MenuItem("Window/Scenes")]
    public static void ShowWindow()
    {
        var window = GetWindow<ScenesEditor>("Scenes");
        window.Show();
    }

    private void OnEnable()
    {
        EditorSceneManager.sceneOpened += OnSceneOpened;
    }

    private void OnDisable()
    {
        EditorSceneManager.sceneOpened -= OnSceneOpened;
    }

    private void CreateGUI()
    {
        var root = rootVisualElement;
        var sceneNames = GetSceneNames();

        _sceneDropdown = new PopupField<string>(sceneNames, 0);
        _sceneDropdown.SetValueWithoutNotify(GetActiveSceneName());
        _sceneDropdown.RegisterValueChangedCallback(OnSceneSelected);

        root.Add(_sceneDropdown);
    }

    private void OnSceneSelected(ChangeEvent<string> evt)
    {
        var selectedScene = evt.newValue;
        SwitchScene(selectedScene);
    }

    private void OnSceneOpened(Scene _, OpenSceneMode __)
    {
        var activeScene = GetActiveSceneName();
        if (_sceneDropdown != null && _sceneDropdown.value != activeScene)
        {
            _sceneDropdown.SetValueWithoutNotify(activeScene);
        }
    }

    private string GetActiveSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }

    private void SwitchScene(string sceneName)
    {
        var scenePath = GetScenePathByName(sceneName);
        if (!string.IsNullOrEmpty(scenePath) && SceneManager.GetActiveScene().path != scenePath)
        {
            EditorSceneManager.OpenScene(scenePath);
        }
    }

    private string GetScenePathByName(string sceneName)
    {
        foreach (var scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled && Path.GetFileNameWithoutExtension(scene.path) == sceneName)
            {
                return scene.path;
            }
        }

        return string.Empty;
    }

    private List<string> GetSceneNames()
    {
        var sceneNames = new List<string>();
        foreach (var scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled)
            {
                sceneNames.Add(Path.GetFileNameWithoutExtension(scene.path));
            }
        }

        return sceneNames;
    }
}

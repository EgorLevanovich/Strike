using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

public class AudioSettings : MonoBehaviour
{
    public static AudioSettings Instance { get; private set; }

    [Header("���������")]
    [SerializeField] private AudioMixerGroup _masterMixerGroup; // ������ ������� ��� ���������� ����������
    private float _currentVolume = 0.75f; // �������� �� ��������� (75%)


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadVolume();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ������������� ��������� ��� ���� �����-���������� � Maps
    public void SetVolume(float volume)
    {
        _currentVolume = volume;

        // ��������� � PlayerPrefs
        PlayerPrefs.SetFloat("MasterVolume", volume);

        // ��������� � ������� (���� ������������)
        if (_masterMixerGroup != null)
        {
            _masterMixerGroup.audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
        }

        // ������� ������ Maps � ������� ����� � ��������� ���������
        GameObject mapsObject = GameObject.Find("Maps");
        if (mapsObject != null)
        {
            AudioSource[] audioSources = mapsObject.GetComponentsInChildren<AudioSource>(true);
            foreach (AudioSource source in audioSources)
            {
                source.volume = _currentVolume;
            }
        }
    }

    private void LoadVolume()
    {
        _currentVolume = PlayerPrefs.GetFloat("MasterVolume", 0.75f);
        SetVolume(_currentVolume); // ��������� ����������� ��������
    }

    public float GetCurrentVolume()
    {
        return _currentVolume;
    }

  

}

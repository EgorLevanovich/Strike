using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maps : MonoBehaviour
{
    private GameObject[] _characters;
    private int _index;
    public GameObject[] _maps;

    public const string SkinKey = "MapsSelected";

    public static bool IsMapBought(int index)
    {
        int value = PlayerPrefs.GetInt("MapBought_" + index, 0);
        Debug.Log("MapBought_" + index + " = " + value);
        return value == 1;
    }

    private void Start()
    {
        foreach (GameObject obj in _maps)
        {
            if (obj != null)
                obj.SetActive(true);
        }
        // Если нет сохранённого выбора, всегда выбираем карту 0
        if (!PlayerPrefs.HasKey(SkinKey))
        {
            PlayerPrefs.SetInt(SkinKey, 0);
        }
        _index = PlayerPrefs.GetInt(SkinKey, 0);
        _characters = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            _characters[i] = transform.GetChild(i).gameObject;
        }
        foreach (GameObject go in _characters)
        {
            go.SetActive(false);
        }
        // Активируем только выбранную карту
        if (_characters[_index])
        {
            _characters[_index].SetActive(true);
        }
    }

    public void SelectLeft()
    {
        _characters[_index].SetActive(false);
        _index--;
        if (_index < 0)
        {
            _index = _characters.Length - 1;
        }
        _characters[_index].SetActive(true);
        Save();
    }

    public void SelectRight()
    {
        _characters[_index].SetActive(false);
        _index++;
        if (_index == _characters.Length)
        {
            _index = 0;
        }
        _characters[_index].SetActive(true);
        Save();
    }

    private void Save()
    {
        Debug.Log($"{_index} {_characters.Length}");
        PlayerPrefs.SetInt(SkinKey, _index);
    }
}


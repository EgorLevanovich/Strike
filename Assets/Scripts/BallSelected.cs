using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSelected : MonoBehaviour
{
    private GameObject[] _Ballscharacters;
    private int _index;

    public const string SkinKey = "BallSelected";

    private void Start()
    {
        _index = PlayerPrefs.GetInt(SkinKey, 0);

        _Ballscharacters = new GameObject[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            _Ballscharacters[i] = transform.GetChild(i).gameObject;
        }
        foreach (GameObject go in _Ballscharacters)
        {
            go.SetActive(false);
        }

        // Загружаем только купленный скин, иначе дефолтный
        if (IsSkinBought(_index))
        {
            _Ballscharacters[_index].SetActive(true);
        }
        else
        {
            _index = 0;
            _Ballscharacters[_index].SetActive(true);
            PlayerPrefs.SetInt(SkinKey, _index);
        }
    }

    public void SelectLeft()
    {
        _Ballscharacters[_index].SetActive(false);
        _index--;
        if (_index < 0)
        {
            _index = _Ballscharacters.Length - 1;
        }
        _Ballscharacters[_index].SetActive(true);
        SaveIfBought();
    }

    public void SelectRight()
    {
        _Ballscharacters[_index].SetActive(false);
        _index++;
        if (_index == _Ballscharacters.Length)
        {
            _index = 0;
        }
        _Ballscharacters[_index].SetActive(true);
        SaveIfBought();
    }

    private void SaveIfBought()
    {
        if (IsSkinBought(_index))
        {
            PlayerPrefs.SetInt(SkinKey, _index);
        }
        // Если не куплен — не сохраняем выбор
    }

    public void SelectSkinByIndex(int index)
    {
        if (index >= 0 && index < _Ballscharacters.Length && IsSkinBought(index))
        {
            _Ballscharacters[_index].SetActive(false);
            _index = index;
            _Ballscharacters[_index].SetActive(true);
            PlayerPrefs.SetInt(SkinKey, _index);
        }
    }

    private bool IsSkinBought(int index)
    {
        return PlayerPrefs.GetInt("BallSkinBought_" + index, index == 0 ? 1 : 0) == 1;
    }
}
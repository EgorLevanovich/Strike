using UnityEngine;

public class PlayerSelect : MonoBehaviour
{
    private GameObject[] _characters;
    private int _index;
    public const string SkinKey = "PlayerSelected";

    private void Start()
    {
        // Создаем индексированный массив персонажей
        _characters = new GameObject[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            _characters[i] = transform.GetChild(i).gameObject;
            _characters[i].SetActive(false);
        }

        // Загрузка последнего выбранного персонажа
        _index = PlayerPrefs.GetInt(SkinKey, 0);

        // Если выбранный скин куплен — загружаем его, иначе загружаем 0
        if (IsSkinBought(_index))
        {
            _characters[_index].SetActive(true);
        }
        else
        {
            _index = 0;
            PlayerPrefs.SetInt(SkinKey, _index);
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
        SaveIfBought();
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

    private bool IsSkinBought(int index)
    {
        return PlayerPrefs.GetInt("SkinBought_" + index, index == 0 ? 1 : 0) == 1;
    }

    // Новый метод: выбрать скин по индексу и сохранить его как выбранный
    public void SelectSkinByIndex(int index)
    {
        if (index >= 0 && index < _characters.Length && IsSkinBought(index))
        {
            _characters[_index].SetActive(false);
            _index = index;
            _characters[_index].SetActive(true);
            PlayerPrefs.SetInt(SkinKey, _index);
        }
    }
}

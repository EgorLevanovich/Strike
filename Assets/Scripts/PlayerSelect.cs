using UnityEngine;

public class PlayerSelect : MonoBehaviour
{
    private GameObject[] _characters;
    private int _index;
    public const string SkinKey = "PlayerSelected";

    // Добавлено: ссылка на RopeWobble
    public RopeWobble ropeWobble;

    private void Start()
    {
        // Создаем индексированный массив персонажей
        _characters = new GameObject[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            _characters[i] = transform.GetChild(i).gameObject;
            _characters[i].SetActive(false);
        }

        // Сортировка: выбранный и купленный персонаж первым, купленные вперед
        ShowPlayerList();

        // Получаем выбранный индекс из PlayerPrefs
        int selectedIndex = PlayerPrefs.GetInt(SkinKey, 0);
        // Если выбранный не куплен, ищем первый купленный
        if (!IsSkinBought(selectedIndex))
        {
            selectedIndex = 0;
            for (int i = 0; i < _characters.Length; i++)
            {
                if (IsSkinBought(i))
                {
                    selectedIndex = i;
                    break;
                }
            }
            PlayerPrefs.SetInt(SkinKey, selectedIndex);
            PlayerPrefs.Save();
        }
        // Находим, где теперь находится выбранный скин в отсортированном массиве
        int newIndex = 0;
        for (int i = 0; i < _characters.Length; i++)
        {
            if (_characters[i].transform.GetSiblingIndex() == selectedIndex)
            {
                newIndex = i;
                break;
            }
        }
        _index = newIndex;
        _characters[_index].SetActive(true);
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
        // Добавлено: дергаем верёвку влево
        if (ropeWobble != null)
            ropeWobble.Wobble(-1f);
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
        // Добавлено: дергаем верёвку вправо
        if (ropeWobble != null)
            ropeWobble.Wobble(1f);
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

    public void ShowPlayerList()
    {
        // Сортируем: сначала купленные, потом некупленные
        System.Array.Sort(_characters, (a, b) => {
            int aIndex = a.transform.GetSiblingIndex();
            int bIndex = b.transform.GetSiblingIndex();
            bool aBought = IsSkinBought(aIndex);
            bool bBought = IsSkinBought(bIndex);
            if (aBought && !bBought) return -1;
            if (!aBought && bBought) return 1;
            return 0;
        });

        // Перемещаем выбранный скин на самый первый
        int selected = PlayerPrefs.GetInt(SkinKey, 0);
        int selectedIndex = -1;
        for (int i = 0; i < _characters.Length; i++)
        {
            int charIndex = _characters[i].transform.GetSiblingIndex();
            if (charIndex == selected)
            {
                selectedIndex = i;
                break;
            }
        }
        if (selectedIndex > 0)
        {
            var temp = _characters[0];
            _characters[0] = _characters[selectedIndex];
            _characters[selectedIndex] = temp;
            _index = 0;
        }
        UpdatePlayerListUI();
    }

    private void UpdatePlayerListUI()
    {
        // Здесь обновите отображение списка персонажей, если нужно (например, активируйте нужные объекты или обновите UI)
    }
}

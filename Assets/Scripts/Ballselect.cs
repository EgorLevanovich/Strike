using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class BallSelectt : MonoBehaviour
{
    [SerializeField] private GameObject[] characters; // Массив объектов персонажей
    private GameObject _player; // Ссылка на текущего активного игрока
    private int index; // Индекс выбранного персонажа

    private void Start()
    {
        // Получаем индекс выбранного скина
        index = PlayerPrefs.GetInt(BallSelected.SkinKey);

        // Активируем только выбранного персонажа
        for (int i = 0; i < characters.Length; i++)
        {
            characters[i].SetActive(i == index);
        }

        // Сохраняем ссылку на активного игрока
        _player = characters[index];
    }
    // Update is called once per frame
    void Update()
    {

    }
}

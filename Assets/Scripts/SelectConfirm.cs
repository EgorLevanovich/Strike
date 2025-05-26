using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectConfirm : MonoBehaviour
{
    public GameObject[] characterPrefabs; // Те же префабы, что и в меню выбора
    public Transform spawnPoint; // Точка спавна в 2D-сцене

    private void Start()
    {
        int selectedCharacter = PlayerPrefs.GetInt("Selected2DCharacter", 0);

        if (characterPrefabs.Length > selectedCharacter && characterPrefabs[selectedCharacter] != null)
        {
            GameObject character = Instantiate(
                characterPrefabs[selectedCharacter],
                new Vector3(spawnPoint.position.x, spawnPoint.position.y, 0), // Z = 0
                Quaternion.identity
            );

            // Дополнительно: настройка сортировки слоев (Sorting Layer)
            SpriteRenderer renderer = character.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                renderer.sortingLayerName = "Characters"; // Укажите ваш слой
                renderer.sortingOrder = 0;
            }
        }
    }
}

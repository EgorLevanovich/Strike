using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectConfirm : MonoBehaviour
{
    public GameObject[] characterPrefabs; // �� �� �������, ��� � � ���� ������
    public Transform spawnPoint; // ����� ������ � 2D-�����

    private void Start()
    {
        int selectedCharacter = PlayerPrefs.GetInt("PlayerSelected", 0);

        if (characterPrefabs.Length > selectedCharacter && characterPrefabs[selectedCharacter] != null)
        {
            GameObject character = Instantiate(
                characterPrefabs[selectedCharacter],
                new Vector3(spawnPoint.position.x, spawnPoint.position.y, 0), // Z = 0
                Quaternion.identity
            );

            // �������������: ��������� ���������� ����� (Sorting Layer)
            SpriteRenderer renderer = character.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                renderer.sortingLayerName = "Characters"; // ������� ��� ����
                renderer.sortingOrder = 0;
            }
        }
    }
}

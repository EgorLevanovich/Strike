using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class BallSelectt : MonoBehaviour
{
    [SerializeField] private GameObject[] characters; // ������ �������� ����������
    private GameObject _player; // ������ �� �������� ��������� ������
    private int index; // ������ ���������� ���������

    private void Start()
    {
        // �������� ������ ���������� �����
        index = PlayerPrefs.GetInt("SelectedBall");
        for (int i = 0; i < characters.Length; i++)
        {
            characters[i].SetActive(i == index);
        }

        //     
        _player = characters[index];
    }
    // Update is called once per frame
    void Update()
    {

    }
}

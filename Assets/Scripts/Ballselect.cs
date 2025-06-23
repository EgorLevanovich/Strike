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

        if (characters != null)
        {
            for (int i = 0; i < characters.Length; i++)
            {
                if(characters[i] != null)
                    characters[i].SetActive(i == index);
            }
            
            if(_player != null)
                _player = characters[index];
        }
    }
}

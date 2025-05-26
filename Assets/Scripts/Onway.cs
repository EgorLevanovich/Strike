using UnityEngine;

public class Onway : MonoBehaviour
{
    [Header("���������")]
    public GameObject[] passthroughObjects; // ������� ������ ������� ����� ������

    private Collider2D _enemyCollider;
    private Rigidbody2D _rb;

    void Start()
    {
        _enemyCollider = GetComponent<Collider2D>();
        _rb = GetComponent<Rigidbody2D>();

        // ���������� ���������� �������� � ���������� ���������
        foreach (var obj in passthroughObjects)
        {
            if (obj != null)
            {
                foreach (var col in obj.GetComponents<Collider2D>())
                {
                    Physics2D.IgnoreCollision(_enemyCollider, col, true);
                }
            }
        }
    }

    // �������������� ������� ��� ���������
    void OnTriggerEnter2D(Collider2D other)
    {
        // ���� ����� �������������� ���������
    }
}
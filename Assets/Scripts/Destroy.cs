using UnityEngine;

public class Destroy : MonoBehaviour
{
    [Tooltip("���������� ���� ������ ��� ������������")]
    public bool destroySelf = true;

    [Tooltip("���������� ������ ������ ��� ������������")]
    public bool destroyOther = true;

    [Tooltip("���� ��������, ������� ����� ���������� (�������� ������ ��� ����� ��������)")]
    public string[] targetTags;
    public AudioSource AudioSource;
    private void OnCollisionEnter(Collision collision)
    {
        ProcessCollision(collision.gameObject);
    }

    // ��� ��������� (���� �����)
    private void OnTriggerEnter(Collider other)
    {
        ProcessCollision(other.gameObject);
    }

    private void ProcessCollision(GameObject other)
    {
        // ��������� ����, ���� ��� ������
        if (targetTags.Length > 0)
        {
            bool tagMatch = false;
            foreach (string tag in targetTags)
            {
                if (other.CompareTag(tag))
                {
                    tagMatch = true;
                    break;
                }
            }
            if (!tagMatch) return;
        }

        // ���������� ������ �������
        if (destroyOther)
        {
            Destroy(other.gameObject);
            AudioSource.Play();
        }

        // ���������� ���� ������
        if (destroySelf)
        {
            Destroy(gameObject);
        }
    }
}
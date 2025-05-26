using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [Header("��������� HP")]
    [SerializeField] private GameObject[] hearts; // ������ ����������� ��������
    [SerializeField] private int maxHP = 3;
    [SerializeField] private string damagingTag = "Border";
    public GameObject _menu;

    private int currentHP;

    private void Start()
    {
        currentHP = maxHP;
        UpdateHeartsUI();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(damagingTag))
        {
            TakeDamage();
        }
    }

    private void TakeDamage()
    {
        if (currentHP > 0)
        {
            currentHP--;
            UpdateHeartsUI();

            if (currentHP <= 0)
            {
                Die();
            }
        }
    }

    private void UpdateHeartsUI()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].SetActive(i < currentHP);
        }
    }

    private void Die()
    {
        // ����� ����� �������� ������� ������
        Destroy(gameObject);
        Time.timeScale = 0f;
        // ��� ������� ������������ ������
        // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
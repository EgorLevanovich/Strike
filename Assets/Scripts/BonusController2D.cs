using UnityEngine;

public class BonusController2D : MonoBehaviour
{
    // 0 - Double Ball (белый), 1 - Homing (красный), 2 - Double Points (жёлтый)
    // [SerializeField] private Color[] bonusColors = new Color[3] { Color.white, Color.red, Color.yellow };

    private BonusSystem2D bonusSystem;
    private BonusType bonusType;
    private bool wasCollected = false;
    private SpriteRenderer spriteRenderer;
    [Header("Audio")]
    public AudioSource collectSound;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Setup(BonusSystem2D system, BonusType type)
    {
        bonusSystem = system;
        bonusType = type;
        // Цвет не трогаем, чтобы можно было настраивать вручную или через BonusSystem2D
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!wasCollected && other.CompareTag("Ball") && bonusSystem != null)
        {
            if (collectSound != null)
                collectSound.Play();
            wasCollected = true;
            bonusSystem.OnBonusCollected(other.gameObject, bonusType);
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Walls"))
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), collision.collider);
        }
        else if (collision.gameObject.CompareTag("Ground"))
        {
            if (bonusSystem != null)
                bonusSystem.OnBonusMissed();
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Ball"))
        {
            // Уничтожаем бонус при столкновении с мячиком
            if (!wasCollected && bonusSystem != null)
            {
                if (collectSound != null)
                    collectSound.Play();
                wasCollected = true;
                bonusSystem.OnBonusCollected(collision.gameObject, bonusType);
                Destroy(gameObject);
            }
        }
    }
} 
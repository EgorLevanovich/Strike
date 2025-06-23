using UnityEngine;

public class RopeWobble : MonoBehaviour
{
    public float wobbleAmount = 20f; // максимальный угол колебания (в градусах)
    public float wobbleDamping = 0.4f; // время затухания

    private float currentWobble = 0f;
    private float wobbleVelocity = 0f;
    private RectTransform rect;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    void Update()
    {
        currentWobble = Mathf.SmoothDamp(currentWobble, 0, ref wobbleVelocity, wobbleDamping);

        if (rect != null)
        {
            rect.localRotation = Quaternion.Euler(0, 0, currentWobble);
        }
        
    }

    // Вызывайте этот метод при перелистывании карт
    public void Wobble(float direction = 1f)
    {
        currentWobble = wobbleAmount * Mathf.Sign(direction);
    }
} 
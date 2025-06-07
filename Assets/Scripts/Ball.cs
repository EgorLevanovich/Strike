using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Ball : MonoBehaviour
{
    private int[] Goal = new int[2];
    public Text _myGoal;
    public AudioSource _ballBounsing;
    public AudioSource _ballBoundry;
    public AudioSource destroyAudio;
    
    private Rigidbody2D rb;
    private Vector2 lastVelocity;
    private float _bounceForce;

    [Header("Physics Settings")]
    public float bounciness = 0.9f; // Коэффициент сохранения энергии при отскоке
    public float friction = 0.98f;  // Трение (замедление)
    public float minVelocity = 0.2f; // Минимальная скорость, ниже которой шарик останавливается

    private void Update()
    {
        lastVelocity = rb.velocity;
        // Реалистичное трение
        if (rb.velocity.magnitude > minVelocity)
        {
            rb.velocity *= friction;
        }
        else if (rb.velocity.magnitude > 0)
        {
            rb.velocity = Vector2.zero;
        }
        //_myGoal.text = Goal[0].ToString();
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Звук для стен
        if (collision.gameObject.CompareTag("Walls"))
        {
            if (_ballBounsing != null)
                _ballBounsing.Play();
        }

        // Голы
        if (collision.gameObject.CompareTag("Border"))
        {
            Goal[0] += 1;
        }

        // Звук для врага
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (_ballBoundry != null)
                _ballBoundry.Play();
        }

        // Реалистичный отскок от платформы и стен
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Walls"))
        {
            float speed = lastVelocity.magnitude;
            Vector2 direction = Vector2.Reflect(lastVelocity.normalized, collision.contacts[0].normal);
            rb.velocity = direction * speed * bounciness;
        }

        // Уничтожение шарика (пример: если столкнулся с Enemy или Ground)
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Ground"))
        {
            if (destroyAudio != null)
                destroyAudio.Play();
            Destroy(gameObject);
        }
    }
}


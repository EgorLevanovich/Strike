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
    
    private Rigidbody2D rb;
    private Vector2 lastVelocity;
    private float _bounceForce;

    private void Update()
    {
        lastVelocity = rb.velocity;
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

        // Отскок от платформы и стен
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Walls"))
        {
            float speed = lastVelocity.magnitude;
            Vector2 direction = Vector2.Reflect(lastVelocity.normalized, collision.contacts[0].normal);
            rb.velocity = direction * speed * 0.95f;
        }
    }
}


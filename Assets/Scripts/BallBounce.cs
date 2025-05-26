using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBounce : MonoBehaviour
{

    private Rigidbody2D _rigidbody2D;
    public float _bounceForce = 10;
    public float _speed = 10f;
    public float _sideBounce;
    public AudioSource bounceAudio;
    // Start is called before the first frame update
    void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
       
        _rigidbody2D.velocity = new Vector2(0f,-_speed);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            float currentSpeed = _rigidbody2D.velocity.magnitude;
            Vector2 normal = collision.contacts[0].normal;
            Vector2 reflectedDirection = Vector2.Reflect(_rigidbody2D.velocity.normalized, normal);
            _rigidbody2D.velocity = reflectedDirection * currentSpeed;
            _rigidbody2D.velocity += new Vector2(0f, _bounceForce);
        }
        else if (collision.gameObject.CompareTag("Walls"))
        {
            Vector2 normal = collision.contacts[0].normal;
            _rigidbody2D.velocity = Vector2.Reflect(_rigidbody2D.velocity, normal);
            _rigidbody2D.velocity += new Vector2(normal.x * _sideBounce, 0f);
            bounceAudio.Play();
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            Vector2 normal = collision.contacts[0].normal;
            _rigidbody2D.velocity = Vector2.Reflect(_rigidbody2D.velocity, normal);
            _rigidbody2D.velocity += new Vector2(normal.x * _sideBounce, 2f);
        }
    }
    // Update is called once per frame
   
}

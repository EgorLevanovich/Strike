using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallsSpeed : MonoBehaviour
{
    public float maxSpeed = 20f;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }
}

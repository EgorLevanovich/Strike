using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scoring : MonoBehaviour
{

    public AudioSource _pop;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            NewBehaviourScript.Instance.AddDestroyedBall();
            Destroy(gameObject);
            _pop.Play();
        }

        if (collision.gameObject.CompareTag("Ground"))
        {
            NewBehaviourScript.Instance.AddDestroyedBall();
            Destroy(gameObject);
            _pop.Play();
        }
    }
}

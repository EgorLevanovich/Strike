using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class balloons : MonoBehaviour
{
    public float _speed;
    public System.Action<GameObject> OnEnemyDestroyed;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
    }
    

}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SoundsButtons : MonoBehaviour
{
    [SerializeField] private bool isOnButton;
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(onClick);
    }

    private void onClick()
    {
       
    }
    void Update()
    {
        
    }
}

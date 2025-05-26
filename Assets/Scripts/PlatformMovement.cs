using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMovement : MonoBehaviour
{
    public float _speed = 10f;
    public Rigidbody2D _rb;
    [SerializeField] private Rigidbody2D[] _players;
    private Vector3 _mouseOffset;
    private bool _isDragging = false;
    private float fixedY;
    [SerializeField] private float minX = -387f;
    [SerializeField] private float maxX = 1087f;
    private Vector2 _targetPosition;

    void Start()
    {
        fixedY = transform.position.y;
        int index = PlayerPrefs.GetInt(PlayerSelect.SkinKey);
        for (int i = 0; i < _players.Length; i++)
        {
            if (i == index)
            {
                _rb = _players[i];
                _rb.gameObject.SetActive(true);
            }
            else
            {
                _players[i].gameObject.SetActive(false);
            }
        }
    }

    void OnMouseDown()
    {
        _isDragging = true;
        Vector3 mouseWorldPos = GetMouseWorldPos();
        _mouseOffset = transform.position - mouseWorldPos;
        _mouseOffset.y = 0;
    }

    void OnMouseUp()
    {
        _isDragging = false;
    }

    void Update()
    {
        if (_isDragging)
        {
            Vector3 newPosition = GetMouseWorldPos() + _mouseOffset;
            newPosition = new Vector3(
                Mathf.Clamp(newPosition.x, minX, maxX),
                fixedY,
                transform.position.z
            );
            _targetPosition = new Vector2(newPosition.x, newPosition.y);
        }
    }

    void FixedUpdate()
    {
        if (_isDragging)
        {
            _rb.MovePosition(_targetPosition);
        }
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = 0;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
}





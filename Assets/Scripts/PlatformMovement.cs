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
    private Collider2D _platformCollider;

    void Start()
    {
        fixedY = transform.position.y;
        _platformCollider = GetComponent<Collider2D>();
        if (_platformCollider == null)
        {
            Debug.LogError("На платформе отсутствует коллайдер!");
        }
        else
        {
            Debug.Log($"Тип коллайдера платформы: {_platformCollider.GetType().Name}");
            Debug.Log($"Размер коллайдера: {_platformCollider.bounds.size}");
        }

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
        Vector3 mouseWorldPos = GetMouseWorldPos();
        Debug.Log($"Нажатие на позицию: {mouseWorldPos}");
        Debug.Log($"Позиция платформы: {transform.position}");
        Debug.Log($"Границы коллайдера: {_platformCollider.bounds}");

        if (_platformCollider != null && _platformCollider.OverlapPoint(mouseWorldPos))
        {
            Debug.Log("Нажатие внутри коллайдера");
            _isDragging = true;
            _mouseOffset = transform.position - mouseWorldPos;
            _mouseOffset.y = 0;
        }
        else
        {
            Debug.Log("Нажатие вне коллайдера");
        }
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
            _rb.position = new Vector2(newPosition.x, newPosition.y);
        }
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = 0;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") && gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}





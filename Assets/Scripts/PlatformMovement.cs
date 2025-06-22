using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMovement : MonoBehaviour
{
    public float _speed = 10f;
    public Rigidbody2D _rb;
    [SerializeField] public Rigidbody2D[] _players;
    private Vector3 _mouseOffset;
    private bool _isDragging = false;
    private float fixedY;
    [SerializeField] private float minX = -387f;
    [SerializeField] private float maxX = 1087f;
    private Vector2 _targetPosition;
    private bool _mouseHeld = false;
    private Vector3 _lastMousePosition;
    private float _dragThreshold = 0.01f; // Минимальное смещение для старта движения
    private Vector2 _touchStartPos;
    private bool _touchHeld = false;
    private bool _touchDragging = false;

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
        _mouseHeld = true;
        _lastMousePosition = Input.mousePosition;
        _isDragging = false;
        _mouseOffset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    void OnMouseUp()
    {
        _mouseHeld = false;
        _isDragging = false;
    }

#if UNITY_ANDROID || UNITY_IOS
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 touchWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, 0));
            touchWorldPos.z = 0;
            if (touch.phase == TouchPhase.Began)
            {
                Collider2D col = GetComponent<Collider2D>();
                if (col != null && col.OverlapPoint(touchWorldPos))
                {
                    _touchHeld = true;
                    _touchDragging = false;
                    _touchStartPos = touch.position;
                }
            }
            if (_touchHeld)
            {
                if (!_touchDragging)
                {
                    if ((touch.position - _touchStartPos).sqrMagnitude > 10f) // Порог в пикселях
                    {
                        _touchDragging = true;
                    }
                }
                if (_touchDragging)
                {
                    float clampedX = Mathf.Clamp(touchWorldPos.x, minX, maxX);
                    _targetPosition = new Vector2(clampedX, fixedY);
                }
            }
            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                _touchHeld = false;
                _touchDragging = false;
            }
        }
        else
        {
            _touchHeld = false;
            _touchDragging = false;
        }
    }
#else
    void Update()
    {
        if (_mouseHeld)
        {
            if (!_isDragging)
            {
                if ((Input.mousePosition - _lastMousePosition).sqrMagnitude > _dragThreshold * _dragThreshold)
                {
                    _isDragging = true;
                }
            }
            if (_isDragging)
            {
                Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition) + _mouseOffset;
                mouseWorldPos.z = 0;
                float clampedX = Mathf.Clamp(mouseWorldPos.x, minX, maxX);
                _targetPosition = new Vector2(clampedX, fixedY);
            }
        }
    }
#endif

    void FixedUpdate()
    {
#if UNITY_ANDROID || UNITY_IOS
        if (_touchDragging)
        {
            _rb.MovePosition(_targetPosition);
        }
#else
        if (_isDragging)
        {
            _rb.MovePosition(_targetPosition);
        }
#endif
    }
}





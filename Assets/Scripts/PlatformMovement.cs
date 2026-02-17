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

    private Camera _camera;

    private void Start()
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
        
        _camera = Camera.main;
    }

    private void OnMouseDown()
    {
        _mouseHeld = true;
        _lastMousePosition = Input.mousePosition;
        _isDragging = false;
        _mouseOffset = transform.position - _camera.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnMouseUp()
    {
        _mouseHeld = false;
        _isDragging = false;
    }
    
    private void Update()
    {
        // ===== TOUCH =====
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            HandlePointer(touch.position, 
                touch.phase == TouchPhase.Began,
                touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled);
        }
        // ===== MOUSE (для редактора) =====
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                HandlePointer(Input.mousePosition, true, false);
            }
            else if (Input.GetMouseButton(0))
            {
                HandlePointer(Input.mousePosition, false, false);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                HandlePointer(Input.mousePosition, false, true);
            }
            else
            {
                _touchHeld = false;
                _touchDragging = false;
            }
        }
    }

    private void HandlePointer(Vector2 screenPos, bool pointerDown, bool pointerUp)
    {
        Vector3 worldPos = _camera.ScreenToWorldPoint(screenPos);
        worldPos.z = 0;

        if (pointerDown)
        {
            Collider2D col = GetComponent<Collider2D>();
            if (col != null && col.OverlapPoint(worldPos))
            {
                _touchHeld = true;
                _touchDragging = false;
                _touchStartPos = screenPos;
            }
        }

        if (_touchHeld)
        {
            if (!_touchDragging)
            {
                if ((screenPos - _touchStartPos).sqrMagnitude > 10f)
                {
                    _touchDragging = true;
                }
            }

            if (_touchDragging)
            {
                var leftBound = _camera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x;
                var rightBound = _camera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;

                var clampedX = Mathf.Clamp(worldPos.x, leftBound, rightBound);
                _targetPosition = new Vector2(clampedX, fixedY);
            }
        }

        if (pointerUp)
        {
            _touchHeld = false;
            _touchDragging = false;
        }
    }
    
    private void FixedUpdate()
    {
        if (_touchDragging)
        {
            _rb.MovePosition(_targetPosition);
        }
    }
}





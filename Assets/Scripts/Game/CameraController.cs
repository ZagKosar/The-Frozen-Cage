using Scripts.App;
using Scripts.Game;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private Transform _body;
    [SerializeField] private CapsuleCollider _playerCollider;

    [Header("Camera")]
    [SerializeField] private float _bodyMaxHeight = 2f;
    [SerializeField] private float _cameraHeightOffset = -0.2f;
    [SerializeField] private float _cameraSmooth = 10f;

    [Header("HeadBob")]
    [SerializeField] private float _amplitude = 0.05f;
    [SerializeField] private float _bobSmooth = 10;
    [SerializeField] private float _frequency = 5;

    private GameTime _gameTime;
    private float _bobTimer = 0;
    private bool _isMoving;

    void Start()
    {
        _gameTime = DependencyContainer.GameTime;

        var inputHandler = DependencyContainer.InputHandler;

        if (_camera == null)
            _camera = Camera.main;

        _camera.transform.position = transform.position;
        _camera.transform.parent = _body;

        inputHandler.OnLook += OnLook;
        inputHandler.OnMove += OnMove;
    }

    void Update()
    {
        UpdateCameraHeight();
    }

    private void UpdateCameraHeight()
    {
        if (_playerCollider == null) return;

        float targetY = _playerCollider.height / _bodyMaxHeight + _cameraHeightOffset;
        var bobOffset = 0f;

        if (_isMoving)
        {
            _bobTimer += _gameTime.DeltaTime * _frequency;
            bobOffset = Mathf.Sin(_bobTimer) * _amplitude;
        }
        else
        {
            _bobTimer = 0;
        }

        targetY += bobOffset;
        var localPos = _camera.transform.localPosition;

        localPos.y = Mathf.Lerp(localPos.y, targetY, _cameraSmooth * _gameTime.DeltaTime);

        _camera.transform.localPosition = localPos;
    }

    private void OnLook(Vector2 delta)
    {
        var settings = DependencyContainer.ClientSettings;

        var xRotation = _camera.transform.localRotation.eulerAngles.x;

        if (xRotation > 180)
            xRotation -= 360;

        var deltaX = delta.x * settings.GameSettings.MouseSensitivity * _gameTime.DeltaTime * 100;
        var deltaY = delta.y * settings.GameSettings.MouseSensitivity * _gameTime.DeltaTime * 100;

        xRotation -= deltaY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        _body.Rotate(Vector3.up * deltaX);
        _camera.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
    }

    private void OnMove(Vector2 direction)
    {
        _isMoving = direction != Vector2.zero;
    }

    public void SetMouseLock(bool isLocked)
    {
        Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !isLocked;
    }

    private void OnDestroy()
    {
        var inputHandler = DependencyContainer.InputHandler;
        if (inputHandler == null) return;

        inputHandler.OnLook -= OnLook;
        inputHandler.OnMove -= OnMove;
    }
}
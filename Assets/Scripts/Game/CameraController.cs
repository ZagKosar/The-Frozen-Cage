using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private Transform _body;

    void Start()
    {
        var inputHandler = DependencyContainer.InputHandler;

        _camera = Camera.main;

        _camera.transform.position = transform.position;
        _camera.transform.parent = _body;

        inputHandler.OnLook += OnLook;
    }

    void Update()
    {
        
    }

    private void OnLook(Vector2 delta)
    {
        var settings = DependencyContainer.ClientSettings;
        var time = DependencyContainer.GameTime;
        var xRotation = _camera.transform.localRotation.x;
        

        var deltaX = delta.x * settings.GameSettings.MouseSensitivity * time.DeltaTime;
        var deltaY = delta.y * settings.GameSettings.MouseSensitivity * time.DeltaTime;

        xRotation -= deltaY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        _body.Rotate(Vector3.up * deltaX);
    }
}

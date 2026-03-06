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
        Debug.Log(delta);
        var settings = DependencyContainer.ClientSettings;
        var time = DependencyContainer.GameTime;
        var xRotation = _camera.transform.localRotation.eulerAngles.x;

        if (xRotation > 180)
            xRotation -= 360;
        

        var deltaX = delta.x * settings.GameSettings.MouseSensitivity * time.DeltaTime * 100;
        var deltaY = delta.y * settings.GameSettings.MouseSensitivity * time.DeltaTime * 100;

        xRotation -= deltaY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);
        Debug.Log(deltaX);
        Debug.Log(deltaY);
        Debug.Log(xRotation);

        _body.Rotate(Vector3.up * deltaX);
        _camera.transform.localRotation =Quaternion.Euler(xRotation, 0, 0); 

        
    }

    public void SetMouseLock(bool isLocked)
    {
        Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !isLocked;
    }
}

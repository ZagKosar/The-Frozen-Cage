using Scripts.WindowSwitcher;
using UnityEngine;

public class AppManager : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    void Start()
    {
        WindowSwitcher.Instance.Initialize();

        OpenWindow(new UIEvents.OpenWindow() { Name = "main_menu_panel" });

        EventManager.Instance.Subscribe<UIEvents.OpenWindow>(OpenWindow);
    }

    void Update()
    {
        
    }

    private void OpenWindow(UIEvents.OpenWindow data)
    {
        WindowSwitcher.Instance.ShowWindow(data.Name);
    }
}

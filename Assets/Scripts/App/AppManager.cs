using Scripts.WindowSwitcher;
using UnityEngine;

public class AppManager : MonoBehaviour
{
    [SerializeField] private WindowSwitcher _windowSwitcher;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    void Start()
    {
        _windowSwitcher.Initialize();

        OpenWindow(new UIEvents.OpenWindow() { Name = "main_menu_panel" });

        EventManager.Instance.Subscribe<UIEvents.OpenWindow>(OpenWindow);
        EventManager.Instance.Subscribe<UIEvents.CloseLastWindow>(CloseLastWindow);
    }

    void Update()
    {
        
    }

    private void OpenWindow(UIEvents.OpenWindow data)
    {
        _windowSwitcher.ShowWindow(data.Name);
    }

    private void CloseLastWindow(UIEvents.CloseLastWindow data)
    {
        _windowSwitcher.CloseLast();
    }
}

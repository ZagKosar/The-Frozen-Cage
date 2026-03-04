using Scripts.WindowSwitcher;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UIEvents;

public class AppManager : MonoBehaviour
{
    [SerializeField] private WindowSwitcher _windowSwitcher;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        DontDestroyOnLoad(Camera.main);
    }

    void Start()
    {
        var clientSettings = DependencyContainer.ClientSettings;
        var graphicsMaster = DependencyContainer.GraphicsMaster;
        var audioMaster = DependencyContainer.AudioMaster;

        DependencyContainer.Instance.Initialize();

        clientSettings.Load();

        _windowSwitcher.Initialize();

        graphicsMaster.Initialize();
        audioMaster.Initialize();

        audioMaster.PlaySound("MainMenu");

        OpenWindow(new UIEvents.OpenWindow() { Name = "main_menu_panel" });

        EventManager.Instance.Subscribe<UIEvents.OpenWindow>(OpenWindow);
        EventManager.Instance.Subscribe<UIEvents.CloseWindow>(CloseWindow);
        EventManager.Instance.Subscribe<UIEvents.CloseLastWindow>(CloseLastWindow);
        EventManager.Instance.Subscribe<UIEvents.QuitGame>(QuitGame);
        EventManager.Instance.Subscribe<UIEvents.StartNewGame>(StartNewGame);
    }

    void Update()
    {
        
    }

    private void OpenWindow(UIEvents.OpenWindow data)
    {
        _windowSwitcher.ShowWindow(data.Name);
    }

    private void CloseWindow(UIEvents.CloseWindow data)
    {
        _windowSwitcher.CloseWindow(data.Name);
    }

    private void CloseLastWindow(UIEvents.CloseLastWindow data)
    {
        _windowSwitcher.CloseLast();
    }

    private void QuitGame(UIEvents.QuitGame data)
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }

    private void StartNewGame(UIEvents.StartNewGame data)
    {
        SceneManager.LoadScene(1);
    }
}

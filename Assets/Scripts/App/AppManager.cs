using Cysharp.Threading.Tasks;
using Scripts.App.Constants;
using Scripts.Events.App;
using Scripts.Game.Save;
using Scripts.WindowSwitcher;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UIEvents;

public class AppManager : MonoBehaviour
{
    [SerializeField] private WindowSwitcher _windowSwitcher;
    
    private SaveSystem _saveSystem = new();

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
        EventManager.Instance.Subscribe<UIEvents.OpenWindowWithContext>(OpenWindowWithContext);
        EventManager.Instance.Subscribe<UIEvents.CloseWindow>(CloseWindow);
        EventManager.Instance.Subscribe<UIEvents.CloseLastWindow>(CloseLastWindow);
        EventManager.Instance.Subscribe<UIEvents.QuitGame>(QuitGame);
        EventManager.Instance.Subscribe<UIEvents.StartNewGame>(StartNewGame);
        EventManager.Instance.Subscribe<UIEvents.ExitToMainMenu>(ExitToMainMenu);
        EventManager.Instance.Subscribe<AppEvents.Save>(SaveGame);
        EventManager.Instance.Subscribe<AppEvents.Load>(LoadGame);

        LoadScene(1).Forget();
    }

    void Update()
    {
        
    }

    private void OpenWindow(UIEvents.OpenWindow data)
    {
        _windowSwitcher.ShowWindow(data.Name);
    }

    private void OpenWindowWithContext(UIEvents.OpenWindowWithContext data)
    {
        _windowSwitcher.ShowWindow(data.Name, context: data.Context);
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
        EventManager.Instance.Invoke(new UIEvents.OpenWindow() { Name = Constants.LoadingWindow });

        LoadScene(2).ContinueWith(() => EventManager.Instance.Invoke(new UIEvents.CloseWindow() { Name = Constants.LoadingWindow })).Forget();
    }

    private void ExitToMainMenu(UIEvents.ExitToMainMenu data)
    {
        CloseGameMenu();
        DontDestroyCamera();

        EventManager.Instance.Invoke(new UIEvents.OpenWindow() { Name = Constants.LoadingWindow});

        LoadScene(1)
            .ContinueWith(() =>
            {
                EventManager.Instance.Invoke(new UIEvents.CloseWindow() { Name = Constants.LoadingWindow });
                EventManager.Instance.Invoke(new UIEvents.OpenWindow() { Name = Constants.MainMenuWindow });
            })
            .Forget();
    }

    private void CloseMainMenu()
    {
        EventManager.Instance.Invoke(new UIEvents.CloseWindow() { Name = Constants.MainMenuWindow });
        EventManager.Instance.Invoke(new UIEvents.CloseWindow() { Name = Constants.SettingsPopUp });
        EventManager.Instance.Invoke(new UIEvents.CloseWindow() { Name = Constants.SaveWindow });
    }

    private void CloseGameMenu()
    {
        EventManager.Instance.Invoke(new UIEvents.CloseWindow() { Name = Constants.PauseWindow });
        EventManager.Instance.Invoke(new UIEvents.CloseWindow() { Name = Constants.InventoryWindow });
        EventManager.Instance.Invoke(new UIEvents.CloseWindow() { Name = Constants.PlayerGUI });
    }

    private void DontDestroyCamera()
    {
        Camera.main.transform.parent = null;
        DontDestroyOnLoad(Camera.main);
    }

    private void SaveGame(AppEvents.Save data)
    {
        _saveSystem.Save(data.Slot);
    }

    private void LoadGame(AppEvents.Load data)
    {
        CloseMainMenu();
        CloseGameMenu();
        DontDestroyCamera();

        EventManager.Instance.Invoke(new UIEvents.OpenWindow() { Name = Constants.LoadingWindow });

        LoadScene(2)
            .ContinueWith(() =>
            {
                LoadSave(data.Slot);

                EventManager.Instance.Invoke(new UIEvents.CloseWindow() { Name = Constants.LoadingWindow });
            })
            .Forget();
    }

    private async UniTask LoadScene(int scene)
    {
        await SceneManager.LoadSceneAsync(scene);
    }

    private void LoadSave(int slot)
    {
        _saveSystem.Load(slot);
    }
}

using Scripts.Events.App;
using Scripts.Windows.Save;
using Scripts.WindowSwitcher;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuWindow : WindowPanel
{
    [SerializeField] private Button _startNewGameButton;
    [SerializeField] private Button _continueGameButton;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _quitGameButton;

    public override void Load()
    {
        
    }

    public override void Destroy()
    {

    }

    public override void Open(object context = null)
    {
        _startNewGameButton.onClick.AddListener(StartNewGame);
        _continueGameButton.onClick.AddListener(ContinueGame);
        _settingsButton.onClick.AddListener(OpenSettings);
        _quitGameButton.onClick.AddListener(OnQuit);

        gameObject.SetActive(true);
    }

    public override void Close()
    {
        _startNewGameButton.onClick.RemoveListener(StartNewGame);
        _continueGameButton.onClick.RemoveListener(ContinueGame);
        _settingsButton.onClick.RemoveListener(OpenSettings);
        _quitGameButton.onClick.RemoveListener(OnQuit);

        gameObject.SetActive(false);
    }

    private void StartNewGame()
    {
        EventManager.Instance.Invoke(new UIEvents.CloseWindow() { Name = "main_menu_panel" });
        EventManager.Instance.Invoke(new UIEvents.StartNewGame());
    }

    private void ContinueGame()
    {
        EventManager.Instance.Invoke(new UIEvents.OpenWindowWithContext() { Name = "save_window", Context = new SaveWindowContext() { IsSaving = false } });
    }

    private void OpenSettings()
    {
        EventManager.Instance.Invoke(new UIEvents.OpenWindow() { Name = "settings_panel" });
    }

    private void OnQuit()
    {
        EventManager.Instance.Invoke(new UIEvents.QuitGame());
    }
}

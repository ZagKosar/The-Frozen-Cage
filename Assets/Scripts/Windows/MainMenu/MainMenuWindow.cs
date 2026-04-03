using Scripts.WindowSwitcher;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuWindow : WindowPanel
{
    [SerializeField] private Button _startNewGameButton;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _quitGameButton;

    public override void Load()
    {

    }

    public override void Destroy()
    {

    }

    public override void Open()
    {
        _startNewGameButton.onClick.AddListener(StartNewGame);
        _settingsButton.onClick.AddListener(OpenSettings);
        _quitGameButton.onClick.AddListener(OnQuit);

        gameObject.SetActive(true);
    }

    public override void Close()
    {
        _startNewGameButton.onClick.RemoveListener(StartNewGame);
        _settingsButton.onClick.RemoveListener(OpenSettings);
        _quitGameButton.onClick.RemoveListener(OnQuit);

        gameObject.SetActive(false);
    }

    private void StartNewGame()
    {
        EventManager.Instance.Invoke(new UIEvents.CloseWindow() { Name = "main_menu_panel" });
        EventManager.Instance.Invoke(new UIEvents.StartNewGame());
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

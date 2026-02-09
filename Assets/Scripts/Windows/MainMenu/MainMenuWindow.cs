using Scripts.WindowSwitcher;
using System;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuWindow : WindowPanel
{
    [SerializeField] private Button _settingsButton;

    public override void Open()
    {
        gameObject.SetActive(true);

        _settingsButton.onClick.AddListener(OpenSettings);
    }

    public override void Close()
    {
        gameObject.SetActive(false);
    }

    private void OpenSettings()
    {
        EventManager.Instance.Invoke(new UIEvents.OpenWindow() { Name = "settings_panel" });
    }
}

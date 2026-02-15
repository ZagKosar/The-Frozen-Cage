using Scripts.WindowSwitcher;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPopUp : WindowPanel
{
    [SerializeField] private Button _closeButton;

    public override void Open()
    {
        gameObject.SetActive(true);
        _closeButton.onClick.AddListener(CloseSettings);
    }

    public override void Close()
    {
        _closeButton.onClick.RemoveListener(CloseSettings);
        gameObject.SetActive(false);
    }

    private void CloseSettings()
    {
        EventManager.Instance.Invoke(new UIEvents.CloseLastWindow());
    }
}

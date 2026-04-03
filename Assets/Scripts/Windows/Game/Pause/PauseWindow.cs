using Scripts.Events.Game;
using Scripts.UI;
using Scripts.WindowSwitcher;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseWindow : WindowPanel
{
    [SerializeField] private Button _continueBtn;
    [SerializeField] private Button _saveBtn;
    [SerializeField] private Button _loadBtn;
    [SerializeField] private Button _settingsBtn;
    [SerializeField] private Button _exitBtn;

    [SerializeField] private YesNoPopup _exitConfirmationPopup;

    public override void Open()
    {
        _continueBtn.onClick.AddListener(OnContinue);
        _saveBtn.onClick.AddListener(OnSave);
        _loadBtn.onClick.AddListener(OnLoad);
        _settingsBtn.onClick.AddListener(OnSettings);
        _exitBtn.onClick.AddListener(OnExit);
        _exitConfirmationPopup.Result += CanExit;

        gameObject.SetActive(true);
    }

    public override void Close()
    {
        _continueBtn.onClick.RemoveListener(OnContinue);
        _saveBtn.onClick.RemoveListener(OnSave);
        _loadBtn.onClick.RemoveListener(OnLoad);
        _settingsBtn.onClick.RemoveListener(OnSettings);
        _exitBtn.onClick.RemoveListener(OnExit);
        _exitConfirmationPopup.Result -= CanExit;

        gameObject.SetActive(false);
    }

    public override void Load()
    {
        
    }

    public override void Destroy()
    {
        
    }

    private void OnContinue()
    {
        EventManager.Instance.Invoke(new GameEvent.Pause());
    }

    private void OnSave()
    {

    }

    private void OnLoad()
    {

    }

    private void OnSettings()
    {
        EventManager.Instance.Invoke(new UIEvents.OpenWindow() { Name = "settings_panel" });
    }

    private void OnExit()
    {
        _exitConfirmationPopup.Show();
    }

    private void CanExit(bool result)
    {
        if (result)
            EventManager.Instance.Invoke(new UIEvents.ExitToMainMenu());
        _exitConfirmationPopup.Hide();
    }

}

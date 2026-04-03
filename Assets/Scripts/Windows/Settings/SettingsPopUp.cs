using Scripts.WindowSwitcher;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SettingsPopUp : WindowPanel
{
    [Header("Game")]
    [SerializeField] private Slider _mouseSensitivitySlider;
    [SerializeField] private TMP_Text _mouseSensitivityText;

    [SerializeField] private Slider _textSpeedSlider;
    [SerializeField] private TMP_Text _textSpeedText;

    [SerializeField] private Switcher _subtitlesSwitcher;

    [Header("Graphics")]
    [SerializeField] private TMP_Dropdown _screenModeDropdown;
    [SerializeField] private TMP_Dropdown _resolutionDropdown;

    [SerializeField] private Slider _brightnessGammaSlider;
    [SerializeField] private TMP_Text _brightnessGammaText;

    [SerializeField] private Switcher _vSyncSwitcher;

    [Header("Audio")]
    [SerializeField] private Slider _masterVolumeSlider;
    [SerializeField] private TMP_Text _masterVolumeText;

    [SerializeField] private Slider _musicVolumeSlider;
    [SerializeField] private TMP_Text _musicVolumeText;

    [SerializeField] private Slider _sfxVolumeSlider;
    [SerializeField] private TMP_Text _sfxVolumeText;

    [Header("Utils")]
    [SerializeField] private Button _closeButton;
    [SerializeField] private Button _saveButton;
    [SerializeField] private Button _defaultButton;

    private ClientSettings _lastClientSettings;

    private readonly List<FullScreenMode> _screenModes = new()
    {
        FullScreenMode.FullScreenWindow,
        FullScreenMode.Windowed,
        FullScreenMode.ExclusiveFullScreen
    };

    private struct ResolutionItem
    {
        public int W;
        public int H;
    }

    private readonly List<ResolutionItem> _resolutions = new();

    public override void Load()
    {
        BuildResolutionDropdown();
    }

    public override void Destroy()
    {

    }

    public override void Open()
    {
        _mouseSensitivitySlider.onValueChanged.AddListener(OnMouseSensitivityChanged);
        _textSpeedSlider.onValueChanged.AddListener(OnTextSpeedChanged);
        _subtitlesSwitcher.Switch += OnSubtitlesChanged;

        _screenModeDropdown.onValueChanged.AddListener(OnScreenModeChanged);
        _resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
        _brightnessGammaSlider.onValueChanged.AddListener(OnBrightnessChanged);
        _vSyncSwitcher.Switch += OnVSyncChanged;

        _masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        _musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        _sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);

        _closeButton.onClick.AddListener(CloseSettings);
        _saveButton.onClick.AddListener(SaveSettings);
        _defaultButton.onClick.AddListener(ResetToDefault);

        LoadSettings();

        _lastClientSettings = DependencyContainer.ClientSettings.Clone();

        gameObject.SetActive(true);
    }

    public override void Close()
    {
        _mouseSensitivitySlider.onValueChanged.RemoveListener(OnMouseSensitivityChanged);
        _textSpeedSlider.onValueChanged.RemoveListener(OnTextSpeedChanged);
        _subtitlesSwitcher.Switch -= OnSubtitlesChanged;

        _screenModeDropdown.onValueChanged.RemoveListener(OnScreenModeChanged);
        _resolutionDropdown.onValueChanged.RemoveListener(OnResolutionChanged);
        _brightnessGammaSlider.onValueChanged.RemoveListener(OnBrightnessChanged);
        _vSyncSwitcher.Switch -= OnVSyncChanged;

        _masterVolumeSlider.onValueChanged.RemoveListener(SetMasterVolume);
        _musicVolumeSlider.onValueChanged.RemoveListener(SetMusicVolume);
        _sfxVolumeSlider.onValueChanged.RemoveListener(SetSFXVolume);

        _closeButton.onClick.RemoveListener(CloseSettings);
        _saveButton.onClick.RemoveListener(SaveSettings);
        _defaultButton.onClick.RemoveListener(ResetToDefault);

        gameObject.SetActive(false);

        if (!_lastClientSettings.EqualsTo(DependencyContainer.ClientSettings))
        {
            DependencyContainer.ClientSettings.CopyFrom(_lastClientSettings);

            DependencyContainer.GraphicsMaster.ApplyAll();

            var audio = DependencyContainer.AudioMaster;
            var a = DependencyContainer.ClientSettings.AudioSettings;

            audio.SetMasterVolume(a.MasterVolume);
            audio.SetMusicVolume(a.MusicVolume);
            audio.SetSFXVolume(a.SFXVolume);
        }
    }

    private void BuildResolutionDropdown()
    {
        _resolutions.Clear();

        var options = new List<string>();
        var unique = new HashSet<string>();

        foreach (var r in Screen.resolutions)
        {
            var key = $"{r.width}x{r.height}";
            if (!unique.Add(key))
                continue;

            _resolutions.Add(new ResolutionItem { W = r.width, H = r.height });
            options.Add(key);
        }

        if (_resolutions.Count == 0)
        {
            _resolutions.Add(new ResolutionItem { W = Screen.width, H = Screen.height });
            options.Add($"{Screen.width}x{Screen.height}");
        }

        _resolutions.Reverse();
        options.Reverse();

        var currentOption = options.IndexOf(_resolutionDropdown.itemText.text);

        _resolutionDropdown.ClearOptions();
        _resolutionDropdown.AddOptions(options);
        _resolutionDropdown.value = currentOption == -1 ? 0: currentOption;
    }

    private void LoadSettings()
    {
        var clientSettings = DependencyContainer.ClientSettings;

        // === Game ===
        var gameSettings = clientSettings.GameSettings;

        _mouseSensitivitySlider.SetValueWithoutNotify(gameSettings.MouseSensitivity);
        _mouseSensitivityText.text = gameSettings.MouseSensitivity.ToString("F2");

        _textSpeedSlider.SetValueWithoutNotify(gameSettings.TextSpeed);
        _textSpeedText.text = gameSettings.TextSpeed.ToString("F2");

        _subtitlesSwitcher.SetWithoutNotify(gameSettings.Subtitles);

        // === Graphics ===
        var graphicsSettings = clientSettings.GraphicsSettings;

        int modeIndex = Mathf.Max(0, _screenModes.IndexOf(graphicsSettings.ScreenMode));
        _screenModeDropdown.SetValueWithoutNotify(modeIndex);

        int resIndex = FindResolutionIndex(graphicsSettings.ResolutionWidth, graphicsSettings.ResolutionHeight);
        _resolutionDropdown.SetValueWithoutNotify(resIndex);

        _brightnessGammaSlider.SetValueWithoutNotify(graphicsSettings.Brightness);
        _brightnessGammaText.text = ((graphicsSettings.Brightness - 0.5f) * 4f).ToString("F2");

        _vSyncSwitcher.SetWithoutNotify(graphicsSettings.VSync);

        // === Audio ===
        _masterVolumeSlider.SetValueWithoutNotify(clientSettings.AudioSettings.MasterVolume);
        _masterVolumeText.text = (clientSettings.AudioSettings.MasterVolume * 100).ToString("F0") + "%";

        _musicVolumeSlider.SetValueWithoutNotify(clientSettings.AudioSettings.MusicVolume);
        _musicVolumeText.text = (clientSettings.AudioSettings.MusicVolume * 100).ToString("F0") + "%";

        _sfxVolumeSlider.SetValueWithoutNotify(clientSettings.AudioSettings.SFXVolume);
        _sfxVolumeText.text = (clientSettings.AudioSettings.SFXVolume * 100).ToString("F0") + "%";
    }

    private int FindResolutionIndex(int width, int height)
    {
        for (int i = 0; i < _resolutions.Count; i++)
        {
            if (_resolutions[i].W == width && _resolutions[i].H == height)
                return i;
        }
        return 0;
    }

    //game
    private void OnMouseSensitivityChanged(float value)
    {
        var gameSettings = DependencyContainer.ClientSettings.GameSettings;

        gameSettings.MouseSensitivity = value;

        _mouseSensitivityText.text = value.ToString("F2");
    }

    private void OnTextSpeedChanged(float value)
    {
        var gameSettings = DependencyContainer.ClientSettings.GameSettings;

        gameSettings.TextSpeed = value;

        _textSpeedText.text = value.ToString("F2");
    }

    private void OnSubtitlesChanged(bool value)
    {
        var gameSettings = DependencyContainer.ClientSettings.GameSettings;

        gameSettings.Subtitles = value;
    }

    //video
    private void OnScreenModeChanged(int index)
    {
        index = Mathf.Clamp(index, 0, _screenModes.Count - 1);

        var graphics = DependencyContainer.GraphicsMaster;
        graphics.ApplyScreenMode(_screenModes[index]);

        var g = DependencyContainer.ClientSettings.GraphicsSettings;
        graphics.ApplyResolution(g.ResolutionWidth, g.ResolutionHeight);
    }

    private void OnResolutionChanged(int index)
    {
        index = Mathf.Clamp(index, 0, _resolutions.Count - 1);

        var item = _resolutions[index];
        DependencyContainer.GraphicsMaster.ApplyResolution(item.W, item.H);
    }

    private void OnBrightnessChanged(float value)
    {
        DependencyContainer.GraphicsMaster.ApplyBrightness(value);

        _brightnessGammaText.text = ((value - 0.5f) * 4f).ToString("F2"); 
    }

    private void OnVSyncChanged(bool value)
    {
        DependencyContainer.GraphicsMaster.ApplyVSync(value);
    }

    //Audio
    private void SetMasterVolume(float volume)
    {
        var audioMaster = DependencyContainer.AudioMaster;

        audioMaster.SetMasterVolume(volume);

        _masterVolumeText.text = (volume * 100).ToString("F0") + "%";
    }

    private void SetMusicVolume(float volume)
    {
        var audioMaster = DependencyContainer.AudioMaster;

        audioMaster.SetMusicVolume(volume);

        _musicVolumeText.text = (volume * 100).ToString("F0") + "%";
    }

    private void SetSFXVolume(float volume)
    {
        var audioMaster = DependencyContainer.AudioMaster;

        audioMaster.SetSFXVolume(volume);

        _sfxVolumeText.text = (volume * 100).ToString("F0") + "%";
    }

    private void CloseSettings()
    {
        EventManager.Instance.Invoke(new UIEvents.CloseLastWindow());
    }

    private void SaveSettings()
    {
        var clientSettings = DependencyContainer.ClientSettings;

        _lastClientSettings = clientSettings.Clone();

        clientSettings.Save();
    }

    private void ResetToDefault()
    {
        var defaultSettings = ClientSettings.Default;
        var clientSettings = DependencyContainer.ClientSettings;

        clientSettings.CopyFrom(defaultSettings);

        DependencyContainer.GraphicsMaster.ApplyAll();

        var audio = DependencyContainer.AudioMaster;
        audio.SetMasterVolume(clientSettings.AudioSettings.MasterVolume);
        audio.SetMusicVolume(clientSettings.AudioSettings.MusicVolume);
        audio.SetSFXVolume(clientSettings.AudioSettings.SFXVolume);

        LoadSettings();
    }
}

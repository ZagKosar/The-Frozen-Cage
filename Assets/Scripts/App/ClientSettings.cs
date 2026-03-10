using Newtonsoft.Json;
using System;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]
public class ClientSettings
{
    [SerializeField] private GameSettings _gameSettings = new();
    [SerializeField] private GraphicsSettings _graphicsSettings = new();
    [SerializeField] private AudioSettings _audioSettings = new();
    
    public GameSettings GameSettings => _gameSettings;
    public GraphicsSettings GraphicsSettings => _graphicsSettings;
    public AudioSettings AudioSettings => _audioSettings;

    public static ClientSettings Default => new ClientSettings
    {
        _gameSettings = GameSettings.Default,
        _graphicsSettings = GraphicsSettings.Default,
        _audioSettings = AudioSettings.Default
    };

    public void Save()
    {
        var savePath = Path.Combine(Application.persistentDataPath, "Settings");
        var saveFile = Path.Combine(savePath, "Save.json");

        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }

        var json = JsonConvert.SerializeObject(this);

        File.WriteAllText(saveFile, json);
    }

    public void Load()
    {
        var saveFile = Path.Combine(Application.persistentDataPath, "Settings/Save.json");

        if (!File.Exists(saveFile))
            return;

        var json = File.ReadAllText(saveFile);

        var loaded = JsonConvert.DeserializeObject<ClientSettings>(json);

        if (loaded == null)
            return;

        _gameSettings = loaded._gameSettings ?? new GameSettings();
        _graphicsSettings = loaded._graphicsSettings ?? new GraphicsSettings();
        _audioSettings = loaded._audioSettings ?? new AudioSettings();
    }

    public ClientSettings Clone()
    {
        var clone = new ClientSettings();
        clone._gameSettings = _gameSettings.Clone();
        clone._graphicsSettings = _graphicsSettings.Clone();
        clone._audioSettings = _audioSettings.Clone();
        return clone;
    }

    public void CopyFrom(ClientSettings other)
    {
        if (other == null) throw new ArgumentNullException(nameof(other));

        _gameSettings.CopyFrom(other._gameSettings);
        _graphicsSettings.CopyFrom(other._graphicsSettings);
        _audioSettings.CopyFrom(other._audioSettings);
        
    }

    public bool EqualsTo(ClientSettings other, float epsilon = 0.0001f)
    {
        if (other == null) return true;

        if (_gameSettings == null || other._gameSettings == null) return false;
        if (_graphicsSettings == null || other._graphicsSettings == null) return false;
        if (_audioSettings == null || other._audioSettings == null) return false;

        return _gameSettings.EqualsTo(other._gameSettings, epsilon)
            && _graphicsSettings.EqualsTo(other._graphicsSettings, epsilon)
            && _audioSettings.EqualsTo(other._audioSettings, epsilon);
    }
}

[Serializable]
public class GameSettings
{
    [SerializeField] private float _mouseSensitivity;
    [SerializeField] private float _textSpeed;
    [SerializeField] private bool _subtitles;

    public float MouseSensitivity
    {
        get => _mouseSensitivity;

        set
        {
            value = Mathf.Clamp01(value);
            _mouseSensitivity = value;
        }
    }

    public float TextSpeed
    {
        get => _textSpeed;

        set
        {
            value = Mathf.Clamp01(value);
            _textSpeed = value;
        }
    }

    public bool Subtitles
    {
        get => _subtitles;

        set
        {
            _subtitles = value;
        }
    }

    public static GameSettings Default => new GameSettings
    {
        MouseSensitivity = 0.5f,
        TextSpeed = 0.5f,
        Subtitles = true
    };

    public GameSettings Clone()
    {
        return new GameSettings
        {
            MouseSensitivity = _mouseSensitivity,
            TextSpeed = _textSpeed,
            Subtitles = _subtitles,
        };
    }

    public void CopyFrom(GameSettings other)
    {
        if (other == null) throw new ArgumentNullException(nameof(other));

        MouseSensitivity = other._mouseSensitivity;
        TextSpeed = other._textSpeed;
        Subtitles = other._subtitles;
    }

    public bool EqualsTo(GameSettings other, float epsilon = 0.0001f)
    {
        if (other == null) return false;

        return Mathf.Abs(_mouseSensitivity - other._mouseSensitivity) <= epsilon
               && Mathf.Abs(_textSpeed - other._textSpeed) <= epsilon
               && _subtitles == other._subtitles;
    }
}

[Serializable]
public class GraphicsSettings
{
    [SerializeField] private FullScreenMode _screenMode = FullScreenMode.FullScreenWindow;
    [SerializeField] private int _resolutionWidth = 1280;
    [SerializeField] private int _resolutionHeight = 720;

    [SerializeField] private float _brightness = 0.5f;

    [SerializeField] private bool _vSync = true;

    public FullScreenMode ScreenMode
    {
        get => _screenMode;
        set => _screenMode = value;
    }

    public int ResolutionWidth
    {
        get => _resolutionWidth;
        set => _resolutionWidth = Mathf.Max(320, value);
    }

    public int ResolutionHeight
    {
        get => _resolutionHeight;
        set => _resolutionHeight = Mathf.Max(240, value);
    }

    public float Brightness
    {
        get => _brightness;
        set => _brightness = Mathf.Clamp01(value);
    }

    public bool VSync
    {
        get => _vSync;
        set => _vSync = value;
    }

    public static GraphicsSettings Default => new GraphicsSettings
    {
        ScreenMode = FullScreenMode.FullScreenWindow,
        ResolutionWidth = Screen.currentResolution.width,
        ResolutionHeight = Screen.currentResolution.height,
        Brightness = 0.5f,
        VSync = true
    };

    public GraphicsSettings Clone()
    {
        return new GraphicsSettings
        {
            ScreenMode = _screenMode,
            ResolutionWidth = _resolutionWidth,
            ResolutionHeight = _resolutionHeight,
            Brightness = _brightness,
            VSync = _vSync
        };
    }

    public void CopyFrom(GraphicsSettings other)
    {
        if (other == null) throw new ArgumentNullException(nameof(other));

        ScreenMode = other._screenMode;
        ResolutionWidth = other._resolutionWidth;
        ResolutionHeight = other._resolutionHeight;
        Brightness = other._brightness;
        VSync = other._vSync;
    }

    public bool EqualsTo(GraphicsSettings other, float epsilon = 0.0001f)
    {
        if (other == null) return false;

        return _screenMode == other._screenMode
               && _resolutionWidth == other._resolutionWidth
               && _resolutionHeight == other._resolutionHeight
               && Mathf.Abs(_brightness - other._brightness) <= epsilon
               && _vSync == other._vSync;
    }
}

[Serializable]
public class AudioSettings
{
    [SerializeField] private float _masterVolume;
    [SerializeField] private float _musicVolume;
    [SerializeField] private float _sfxVolume;

    public float MasterVolume
    {
        get => _masterVolume;

        set
        {
            value = Mathf.Clamp01(value);
            _masterVolume = value;
        }
    }

    public float MusicVolume
    {
        get => _musicVolume;

        set
        {
            value = Mathf.Clamp01(value);
            _musicVolume = value;
        }
    }

    public float SFXVolume
    {
        get => _sfxVolume;

        set
        {
            value = Mathf.Clamp01(value);
            _sfxVolume = value;
        }
    }

    public static AudioSettings Default => new AudioSettings
    {
        MasterVolume = 0.8f,
        MusicVolume = 0.8f,
        SFXVolume = 0.8f
    };

    public AudioSettings Clone()
    {
        return new AudioSettings
        {
            MasterVolume = _masterVolume,
            MusicVolume = _musicVolume,
            SFXVolume = _sfxVolume
        };
    }

    public void CopyFrom(AudioSettings other)
    {
        if (other == null) throw new ArgumentNullException(nameof(other));

        MasterVolume = other._masterVolume;
        MusicVolume = other._musicVolume;
        SFXVolume = other._sfxVolume;
    }

    public bool EqualsTo(AudioSettings other, float epsilon = 0.0001f)
    {
        if (other == null) return false;

        return Mathf.Abs(_masterVolume - other._masterVolume) <= epsilon
            && Mathf.Abs(_musicVolume - other._musicVolume) <= epsilon
            && Mathf.Abs(_sfxVolume - other._sfxVolume) <= epsilon;
    }
}
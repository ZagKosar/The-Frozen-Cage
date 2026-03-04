using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GraphicsManager : MonoBehaviour
{
    [Header("Optional (Brightness via URP Volume)")]
    [SerializeField] private Volume _globalVolume;

    private ColorAdjustments _colorAdjustments;
    private GraphicsSettings _graphicsSettings;

    public void Initialize()
    {
        _graphicsSettings = DependencyContainer.ClientSettings.GraphicsSettings;

        if (_globalVolume != null && _globalVolume.profile != null)
            _globalVolume.profile.TryGet(out _colorAdjustments);

        ApplyAll();
    }

    public void ApplyAll()
    {
        ApplyScreenMode(_graphicsSettings.ScreenMode);
        ApplyResolution(_graphicsSettings.ResolutionWidth, _graphicsSettings.ResolutionHeight);
        ApplyVSync(_graphicsSettings.VSync);
        ApplyBrightness(_graphicsSettings.Brightness);
    }

    public void ApplyScreenMode(FullScreenMode mode)
    {
        _graphicsSettings.ScreenMode = mode;
        Screen.fullScreenMode = mode;
    }

    public void ApplyResolution(int width, int height)
    {
        _graphicsSettings.ResolutionWidth = width;
        _graphicsSettings.ResolutionHeight = height;

        Screen.SetResolution(width, height, _graphicsSettings.ScreenMode);
    }

    public void ApplyVSync(bool enabled)
    {
        _graphicsSettings.VSync = enabled;
        QualitySettings.vSyncCount = enabled ? 1 : 0;
    }

    public void ApplyBrightness(float value01)
    {
        _graphicsSettings.Brightness = value01;

        if (_colorAdjustments != null)
        {
            _colorAdjustments.postExposure.value = Mathf.Lerp(-2f, 2f, value01);
        }
    }
}
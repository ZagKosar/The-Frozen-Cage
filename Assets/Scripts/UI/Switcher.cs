using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class Switcher : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private Image _handler;
    [SerializeField] private Color _offColor;
    [SerializeField] private Color _onColor;
    [SerializeField] private Button _button;

    public Action<bool> Switch;

    public bool Value => _isOn;

    private bool _isOn = false;

    private void OnEnable()
    {
        _button.onClick.AddListener(OnClick);
    }
    private void OnDisable()
    {
        _button.onClick.RemoveListener(OnClick);
    }

    public void SetWithoutNotify(bool isOn)
    {
        _isOn = isOn;
        _slider.value = isOn ? 1 : 0;
    }

    private void OnClick()
    {
        if (_isOn)
            PlayOff();
        else
            PlayOn();
    }

    private void PlayOff()
    {
        _slider.DOValue(0f, 0.2f)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                _isOn = false;
                Switch?.Invoke(_isOn);
            });
        _handler.DOColor(_offColor, 0.2f);
    }

    private void PlayOn()
    {
        _slider.DOValue(1f, 0.2f)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                _isOn = true;
                Switch?.Invoke(_isOn);
            });
        _handler.DOColor(_onColor, 0.2f);
    }
}


using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Presenter : MonoBehaviour
{
    [SerializeField] private bool _playOnStart;
    [SerializeField] private List<Sprite> _sprites = new();
    [SerializeField] private int _fps;
    private bool _isPlaying;
    private float _nextUpdateTime;
    private int _currentSprite;
    private Image _image;

    private float DeltaTime => 1f / _fps;

    void Start()
    {
        _image = GetComponent<Image>();

        if (!_playOnStart)
            return;

        _isPlaying = true;
    }

    void Update()
    {
        if (!_isPlaying)
            return;

        if (_sprites.Count <= 0)
            return;

        if (Time.time < _nextUpdateTime)
            return;

        _nextUpdateTime = Time.time + DeltaTime;

        _image.sprite = _sprites[_currentSprite];

        _currentSprite = (_currentSprite + 1) % _sprites.Count;
    }

    public void Play()
    {
        _isPlaying = true;

        _nextUpdateTime = Time.time + DeltaTime;
    }

    public void Stop()
    {
        _isPlaying = false;
    }
}

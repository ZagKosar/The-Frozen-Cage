using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Sources")]
    [SerializeField] private AudioSource _musicSource;

    [Header("Options")]
    [SerializeField] private AudioSettings _audioSettings;

    [Header("Packs")]
    [SerializeField] private List<SoundPack> _packs;

    private Dictionary<string, AudioClip> _clipDictionary = new Dictionary<string, AudioClip>();

    public void Initialize()
    {
        foreach (var pack in _packs)
            _clipDictionary[pack.Name] = pack.AudioClip;

        _audioSettings = DependencyContainer.ClientSettings.AudioSettings;
        _musicSource.volume = _audioSettings.MasterVolume * _audioSettings.MusicVolume;
    }

    public void SetMasterVolume(float volume)
    {
        _audioSettings.MasterVolume = volume;
        _musicSource.volume = _audioSettings.MasterVolume * _audioSettings.MusicVolume;
    }

    public void SetMusicVolume(float volume)
    {
        _audioSettings.MusicVolume = volume;
        _musicSource.volume = _audioSettings.MasterVolume * _audioSettings.MusicVolume;
    }

    public void SetSFXVolume(float volume)
    {
        _audioSettings.SFXVolume = volume;
    }

    public void PlaySound(string name)
    {
        if (!_clipDictionary.TryGetValue(name,out var audioClip))
            return;

        _musicSource.clip = audioClip;

        _musicSource.Play();
    }

    public void StopSound()
    {
        
    }
}

[Serializable]
public class SoundPack
{
    public string Name;
    public AudioClip AudioClip;
}

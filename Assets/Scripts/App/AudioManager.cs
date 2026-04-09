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

    private Dictionary<string, SoundPack> _clipDictionary = new Dictionary<string, SoundPack>();

    public void Initialize()
    {
        DontDestroyOnLoad(this);

        foreach (var pack in _packs)
            _clipDictionary[pack.Name] = pack;

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
        if (!_clipDictionary.TryGetValue(name, out var pack))
        {
            Debug.LogWarning($"[AudioManager] Sound '{name}' not found");
            return;
        }

        _musicSource.clip = pack.AudioClip;
        _musicSource.loop = pack.Loop;
        _musicSource.Play();
    }

    public void StopSound()
    {
        _musicSource.Stop();
        _musicSource.clip = null;
    }

    public void PauseSound()
    {
        _musicSource.Pause();
    }

    public void ResumeSound()
    {
        _musicSource.UnPause();
    }
}

[Serializable]
public class SoundPack
{
    public string Name;
    public AudioClip AudioClip;
    public bool Loop;
}
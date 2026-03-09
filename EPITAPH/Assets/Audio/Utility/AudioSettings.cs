using System;
using System.IO;
using UnityEngine;

// Script that is serializeable in order to save/load the audio settings

[Serializable]
public class AudioSettings
{
    [SerializeField] private float _masterVolume = 0;
    [SerializeField] private float _SFXVolume = 1;
    [SerializeField] private float _musicVolume = 1;
    [SerializeField] private float _ambienceVolume = 1;

    public void Serialize()
    {
        _masterVolume = AudioManager.MasterVolume;
        _SFXVolume= AudioManager.SFXVolume;
        _musicVolume = AudioManager.MusicVolume;
        _ambienceVolume = AudioManager.AmbienceVolume;
    }

    public void Deserialize()
    {
        AudioManager.MasterVolume = _masterVolume;
        AudioManager.SFXVolume = _SFXVolume;
        AudioManager.MusicVolume = _musicVolume;
        AudioManager.AmbienceVolume = _ambienceVolume;
    }


}

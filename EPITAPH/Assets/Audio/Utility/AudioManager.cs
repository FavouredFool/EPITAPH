using FMOD.Studio;
using UnityEngine;
using FMODUnity;
using System;
using static UnityEngine.Rendering.DebugUI;
using System.IO;
using Unity.VisualScripting;

public static class AudioManager
{
    private static float _masterVolume; // locally saving all changes
    private static float _SFXVolume;
    private static float _musicVolume;
    private static float _ambienceVolume;

     static String[] _variableNames = {"VolumeMaster","VolumeMusic","VolumeGame"};



    // Volume Controls:
    // use number between 0 and 1
    public static float MasterVolume
    {   
        get 
        { 
            RuntimeManager.GetBus("bus:/").getVolume(out _masterVolume);
            return _masterVolume;
        }
        set
        {
            _masterVolume = value;
            Bus masterBus = RuntimeManager.GetBus("bus:/");
            masterBus.setVolume(value);
            SaveSettings();
        }
    }


    public static float SFXVolume
    {
        get
        {
            RuntimeManager.GetBus("bus:/SFX").getVolume(out _SFXVolume);
            return _SFXVolume;
        }
        set 
        {
            _SFXVolume = value;
            Bus sfxBus = RuntimeManager.GetBus("bus:/SFX");
            sfxBus.setVolume(value);
            SaveSettings();
        }
    

    }

    public static void SetVolumeLevel(string variableName, int value)
    {
        switch (variableName)
        {
            case "VolumeMaster":
                MasterVolume = (float)value/4.0f;
                return;
            case "VolumeMusic":
                MusicVolume= (float)value / 4.0f;
                return;
            case "VolumeGame":
                SFXVolume = (float)value / 4.0f;
                return;
            default:
                return;

        }
    }


    public static float MusicVolume
    {
        get 
        {
            RuntimeManager.GetBus("bus:/Music").getVolume(out _musicVolume);
            return _musicVolume;
        }
        set
        {
            Bus musicBus = RuntimeManager.GetBus("bus:/Music");
            musicBus.setVolume(value);
            _musicVolume = value;
            SaveSettings();
        }
    }

    public static float AmbienceVolume
    {
        set
        {
            _ambienceVolume = value;
            Bus ambienceBus = RuntimeManager.GetBus("bus:/Ambience");
            ambienceBus.setVolume(value);
            SaveSettings();
        }
        get 
        {
            RuntimeManager.GetBus("bus:/Ambience").getVolume(out _ambienceVolume);
            return _ambienceVolume;
        }
    }

    public static string[] VariableNames { get => _variableNames;}

    public static void LoadSettings()
    {
        _masterVolume = ((float)PlayerPrefs.GetInt(_variableNames[0]))/4.0f;
        _SFXVolume = ((float)PlayerPrefs.GetInt(_variableNames[1]))/4.0f;
        _musicVolume = ((float)PlayerPrefs.GetInt(_variableNames[2]))/4.0f;
    }

    public static void SaveSettings()
    {
        Debug.Log($"{(int)(_masterVolume * 4.0f)},{_masterVolume}");
        PlayerPrefs.SetInt(_variableNames[0], (int)(_masterVolume* 4.0f));
        PlayerPrefs.SetInt(_variableNames[1], (int)(_musicVolume* 4.0f));
        PlayerPrefs.SetInt(_variableNames[2], (int)(_SFXVolume* 4.0f));
        
    }
}


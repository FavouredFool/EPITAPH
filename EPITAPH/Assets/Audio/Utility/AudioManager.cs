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

    // filepath to save the audio settings
    static string AudioSaveFile
    => Path.Combine(Application.persistentDataPath, "AudioSettings.json");


    // Volume Controls:
    // use number between 0 and 1
    public static float MasterVolume
    {   
        get 
        { 
            RuntimeManager.GetBus("bus:/Master").getVolume(out _masterVolume);
            return _masterVolume;
        }
        set
        {
            _masterVolume = value;
            Bus masterBus = RuntimeManager.GetBus("bus:/Master");
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

    public static void LoadSettings()
    {
        var audioSettings = new AudioSettings();
        try
        {
            var json = File.ReadAllText(AudioSaveFile);
            
            JsonUtility.FromJsonOverwrite(json, audioSettings);

        }
        catch
        {
        }
        audioSettings.Deserialize();
    }

    public static void SaveSettings()
    {
        var audioSettings = new AudioSettings();
        audioSettings.Serialize();
        var json = JsonUtility.ToJson(audioSettings); ;
        File.WriteAllText(AudioSaveFile, json);    
    }
}


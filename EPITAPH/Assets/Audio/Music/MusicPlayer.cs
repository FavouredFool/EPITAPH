using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    // Structure to keep track of Music Information
    class TimelineInfo
    {
        public int CurrentMusicBar = 0;
        public int Beat = 0;
        
    }


    TimelineInfo timelineInfo; // Data
    GCHandle timelineHandle; // Weird Handle for the data

    public FMODUnity.EventReference MusicEvent; //

    FMOD.Studio.EVENT_CALLBACK beatCallback;
    FMOD.Studio.EventInstance musicInstance;

    // Make the Music player a singleton for now
    static MusicPlayer instance;

    
    void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("Tried to initialize multiple Music Players");
        }
        instance = this;

        timelineInfo = new TimelineInfo();

        // Explicitly create the delegate object and assign it to a member so it doesn't get freed
        // by the garbage collected while it's being used
        beatCallback = new FMOD.Studio.EVENT_CALLBACK(BeatEventCallback);
        StartMusic();
    }

    void OnDestroy()
    {
        musicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        musicInstance.release();
    }

    public void StartMusic()
    {
        musicInstance = FMODUnity.RuntimeManager.CreateInstance(MusicEvent);

        // Pin the class that will store the data modified during the callback
        timelineHandle = GCHandle.Alloc(timelineInfo);
        // Pass the object through the userdata of the instance
        musicInstance.setUserData(GCHandle.ToIntPtr(timelineHandle));

        musicInstance.setCallback(beatCallback, FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT | FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER);
        musicInstance.start();
    }


    [AOT.MonoPInvokeCallback(typeof(FMOD.Studio.EVENT_CALLBACK))]
    static FMOD.RESULT BeatEventCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
    {
        FMOD.Studio.EventInstance instance = new FMOD.Studio.EventInstance(instancePtr);

        // Retrieve the user data
        IntPtr timelineInfoPtr;
        FMOD.RESULT result = instance.getUserData(out timelineInfoPtr);
        if (result != FMOD.RESULT.OK)
        {
            Debug.LogError("Timeline Callback error: " + result);
        }
        else if (timelineInfoPtr != IntPtr.Zero)
        {
            // Get the object to store beat and marker details
            GCHandle timelineHandle = GCHandle.FromIntPtr(timelineInfoPtr);
            TimelineInfo timelineInfo = (TimelineInfo)timelineHandle.Target;

            switch (type)
            {
                case FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT:
                    {
                        var parameter = (FMOD.Studio.TIMELINE_BEAT_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_BEAT_PROPERTIES));
                        timelineInfo.CurrentMusicBar = parameter.bar;
                        timelineInfo.Beat = parameter.beat;
                        AudioBus.Fire<BeatChanged>(new BeatChanged(parameter.bar, parameter.beat));
                        break;
                    }
                case FMOD.Studio.EVENT_CALLBACK_TYPE.DESTROYED:
                    {
                        // Now the event has been destroyed, unpin the timeline memory so it can be garbage collected
                        timelineHandle.Free();
                        break;
                    }
            }
        }
        return FMOD.RESULT.OK;
    }
}

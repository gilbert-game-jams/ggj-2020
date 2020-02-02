using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [FMODUnity.EventRef] public string backgroundMusicEvent;
    private FMOD.Studio.EventInstance backgroundMusicInstance;

    private void Start()
    {
        backgroundMusicInstance = FMODUnity.RuntimeManager.CreateInstance(backgroundMusicEvent);
        backgroundMusicInstance.start();
    }

    private void OnDestroy()
    {
        backgroundMusicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [FMODUnity.EventRef] public string backgroundMusicEvent;

    private void Start()
    {
        FMOD.Studio.EventInstance backgroundMusicInstance = FMODUnity.RuntimeManager.CreateInstance(backgroundMusicEvent);
        backgroundMusicInstance.start();
    }
}

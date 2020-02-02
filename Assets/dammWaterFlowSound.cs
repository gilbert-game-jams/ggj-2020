using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dammWaterFlowSound : MonoBehaviour
{
    [FMODUnity.EventRef] public string waterFlowEvent;
    private FMOD.Studio.EventInstance waterFlowInstance;

    private void Start()
    {
        waterFlowInstance = FMODUnity.RuntimeManager.CreateInstance(waterFlowEvent);
        waterFlowInstance.start();
    }

    private void OnDestroy()
    {
        waterFlowInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }
}

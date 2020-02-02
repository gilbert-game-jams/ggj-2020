using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrackBehaviour : MonoBehaviour
{
    public delegate void CrackStateChangedDelegate(CrackBehaviour crack, CrackState prevState, CrackState newState);
    public CrackStateChangedDelegate CrackStateChanged = delegate {};

    [FMODUnity.EventRef] public string spawnSoundEvent;
    private FMOD.Studio.EventInstance spawnSoundInstance;
    [FMODUnity.EventRef] public string hitSoundEvent;
    [FMODUnity.EventRef] public string repairCompleteSoundEvent;

    public enum CrackState { Repaired, Open, Nonexistent }

    public GameObject _repairedCrack;
    public GameObject _openCrack;
    bool Taken = false;
    
    [Range(0f, 30f)]
    public float _timeUntilDespawn = 5.0f;

    [HideInInspector]
    public CrackState CurrentState = CrackState.Nonexistent;
    float _timeSinceFixed = 0.0f;
    private void Awake() 
    {
        _repairedCrack.SetActive(false);
        _openCrack.SetActive(false);
    }

    public void SetCrackState(CrackState newState) {
        CrackStateChanged.Invoke(this, CurrentState, newState);

        CurrentState = newState;
        switch(newState) {
            case CrackState.Open:
                _repairedCrack.SetActive(false);
                _openCrack.SetActive(true);
                spawnSoundInstance = FMODUnity.RuntimeManager.CreateInstance(spawnSoundEvent);
                spawnSoundInstance.start();
                break;
            case CrackState.Repaired:
                _repairedCrack.SetActive(true);
                _openCrack.SetActive(false);
                spawnSoundInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                break;
            case CrackState.Nonexistent:
                _repairedCrack.SetActive(false);
                _openCrack.SetActive(false);
                this.gameObject.SetActive(false);
                break;
        }
    }

    private void OnCollisionEnter(Collision other) 
    {
        if (other.gameObject.GetComponent<ArrowBehaviour>() && CurrentState == CrackState.Open) 
        {
            SetCrackState(CrackState.Repaired);
        }
    }

    private void Update() 
    {
        if(CurrentState == CrackState.Repaired) {
            _timeSinceFixed += Time.deltaTime;
            if(_timeSinceFixed > _timeUntilDespawn) {
                FMODUnity.RuntimeManager.PlayOneShot(repairCompleteSoundEvent, transform.position);
                SetCrackState(CrackState.Nonexistent);
            }
        }
    }

    public void UndoRepair()
    {
        Taken = false;
        SetCrackState(CrackState.Open);
    }

    public bool CanTake()
    {
        return CurrentState == CrackState.Repaired && !Taken;
    }

    public void Take()
    {
        Taken = true;
    }
}

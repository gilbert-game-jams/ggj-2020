using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrackBehaviour : MonoBehaviour
{
    [FMODUnity.EventRef] public string spawnSoundEvent;
    private FMOD.Studio.EventInstance spawnSoundInstance;
    [FMODUnity.EventRef] public string hitSoundEvent;
    [FMODUnity.EventRef] public string repairCompleteSoundEvent;

    public enum CrackState { Repaired, Broken }

    public GameObject _fixedCrack;
    public GameObject _brokenCrack;
    bool Taken = false;
    
    [Range(0f, 30f)]
    public float _timeUntilDespawn = 5.0f;

    CrackState _crackState = CrackState.Broken;
    float _timeSinceFixed = 0.0f;
    private void Awake() {
        SetCrackState(CrackState.Broken);
        spawnSoundInstance = FMODUnity.RuntimeManager.CreateInstance(spawnSoundEvent);
    }



    void SetCrackState(CrackState state) {
        _crackState = state;
        switch(state) {
            case CrackState.Broken:
                _fixedCrack.SetActive(false);
                spawnSoundInstance.start();
                _brokenCrack.SetActive(true);
                break;
            case CrackState.Repaired:
                _fixedCrack.SetActive(true);
                spawnSoundInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                _brokenCrack.SetActive(false);
                break;
        }
    }

    private void OnCollisionEnter(Collision other) 
    {
        if (other.gameObject.GetComponent<ArrowBehaviour>()) 
        {
            SetCrackState(CrackState.Repaired);
        }
    }

    private void Update() 
    {
        if(_crackState == CrackState.Repaired) {
            _timeSinceFixed += Time.deltaTime;
            if(_timeSinceFixed > _timeUntilDespawn) {
                FMODUnity.RuntimeManager.PlayOneShot(repairCompleteSoundEvent, transform.position);
                Destroy(gameObject);
            }
        }
    }

    public void UndoRepair()
    {
        SetCrackState(CrackState.Broken);
    }


    public bool CanTake()
    {
        bool canTake = _crackState == CrackState.Repaired && !Taken ? true :false;
        return canTake;
    }

    public void Take()
    {
        Taken = true;
    }

    public void GiveBack()
    {
        Taken = false;
    }
}

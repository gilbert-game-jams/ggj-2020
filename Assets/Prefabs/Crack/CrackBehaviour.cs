using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrackBehaviour : MonoBehaviour
{
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
    }


    void SetCrackState(CrackState state) {
        _crackState = state;
        switch(state) {
            case CrackState.Broken:
                _fixedCrack.SetActive(false);
                _brokenCrack.SetActive(true);
                Taken = false;
                break;
            case CrackState.Repaired:
                _fixedCrack.SetActive(true);
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
                Destroy(gameObject);
            }
        }
    }

    public void UndoRepair()
    {
        SetCrackState(CrackState.Broken);
    }

    public bool IsTaken()
    {
        return Taken;
    }

    public void Take()
    {
        Taken = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrackManager : MonoBehaviour
{
    [SerializeField]
    Transform CrackParent;
    
    [HideInInspector]
    public List<CrackBehaviour> NonexistentCracks = new List<CrackBehaviour>(); 
    
    [HideInInspector]
    public List<CrackBehaviour> OpenCracks = new List<CrackBehaviour>();

    [HideInInspector]
    public List<CrackBehaviour> RepairedCracks = new List<CrackBehaviour>();

    [Range(1, 30)]
    public float _minSpawnTime = 2f;

    [Range(1, 30)]
    public float _maxSpawnTime = 8f;
    float _timeUntilSpawn = 0.0f;

    private void OnValidate() {
        _maxSpawnTime = _maxSpawnTime < _minSpawnTime ? _minSpawnTime : _maxSpawnTime;
    }

    // Start is called before the first frame update
    void Start()
    {
        //add alla the cracks in the list 
        for(int i=0; i< CrackParent.childCount; i++)
        {
            NonexistentCracks.Add(CrackParent.GetChild(i).GetComponent<CrackBehaviour>());
            NonexistentCracks[i].CrackStateChanged += OnCrackStateChanged;
        }
    }

    private void FixedUpdate() 
    {
        _timeUntilSpawn -= Time.deltaTime;
        if(_timeUntilSpawn <= 0.0f) 
        {
            int crackIndex = Random.Range(0, NonexistentCracks.Count - 1);

            var crack = NonexistentCracks[crackIndex];
            crack.SetCrackState(CrackBehaviour.CrackState.Open);

            _timeUntilSpawn = Random.Range(_minSpawnTime, _maxSpawnTime);
        }
    }

    void OnCrackStateChanged(CrackBehaviour crack, CrackBehaviour.CrackState prevState, CrackBehaviour.CrackState newState) {
        switch(prevState) {
            case CrackBehaviour.CrackState.Open:
                OpenCracks.Remove(crack);
                break;
            case CrackBehaviour.CrackState.Repaired:
                RepairedCracks.Remove(crack);
                break;
            case CrackBehaviour.CrackState.Nonexistent:
                NonexistentCracks.Remove(crack);
                break;
        }

        switch(newState) {
            case CrackBehaviour.CrackState.Open:
                OpenCracks.Add(crack);
                break;
            case CrackBehaviour.CrackState.Repaired:
                RepairedCracks.Add(crack);
                break;
            case CrackBehaviour.CrackState.Nonexistent:
                NonexistentCracks.Add(crack);
                break;
        }
    }

    public bool TryGetCrack(out CrackBehaviour _crack)
    {
        _crack = null;
        int randomIndex = Random.Range(0,OpenCracks.Count);
        for(int i = 0; i < OpenCracks.Count; i++)
        {
            if(OpenCracks[randomIndex].CanTake())
            {
                _crack = OpenCracks[randomIndex];
                return true;
            }
            randomIndex++;
            randomIndex = randomIndex%OpenCracks.Count;
        }
        return false;
    }
}

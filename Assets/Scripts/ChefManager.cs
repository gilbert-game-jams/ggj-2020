using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChefManager : MonoBehaviour
{
    public GameObject[] PatrolPoints;

    [SerializeField]
    Chef ChefPrefab;

    [SerializeField]
    Transform [] SpawnPoints;

    [SerializeField]
    int MaxChefs = 10;

    [SerializeField]
    float _spawnTime = 10f;

    int NumberOfChefs = 0;
    List<Chef> _chefs = new List<Chef>();
    float _timeUntilSpawn;

    void Awake() {
        _timeUntilSpawn = _spawnTime;
    }

    void OnChefDied(Chef chef) {
        _chefs.Remove(chef);
        _timeUntilSpawn = _spawnTime;
    }

    private void FixedUpdate()    
    {
        _timeUntilSpawn -= Time.deltaTime;
        if(_timeUntilSpawn < 0 && _chefs.Count < MaxChefs)
        {
            var chef = SpawnChef();
            chef.ChefManager = this;
            chef.ChefDied += OnChefDied;
            _chefs.Add(chef);
            _timeUntilSpawn = _spawnTime;
        }
    }

    Chef SpawnChef()
    {
        int randomIndex = Random.Range(0,SpawnPoints.Length);
        Vector3 SpawnPos =  SpawnPoints[randomIndex].transform.position;
        return GameObject.Instantiate(ChefPrefab, SpawnPos, Quaternion.identity);
    }

}

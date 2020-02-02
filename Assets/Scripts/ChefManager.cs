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
    float MaxSpawnDelay = 20;

    [SerializeField]
    float MinSpawnDelay = 5;

    int NumberOfChefs = 0;
    float SpawnTimer;
    List<Chef> _chefs = new List<Chef>();

    private void Start() 
    {
        UpdateTimer();
    }

    void OnChefDied(Chef chef) {
        _chefs.Remove(chef);
    }

    private void FixedUpdate()    
    {
        if(SpawnTimer < Time.time)
        {
            if(_chefs.Count < MaxChefs)
            {
                var chef = SpawnChef();
                chef.ChefManager = this;
                chef.ChefDied += OnChefDied;
                _chefs.Add(chef);
            }
            UpdateTimer();
        }
    }

    void UpdateTimer()
    {
        SpawnTimer = Time.time + Random.Range(MinSpawnDelay,MaxSpawnDelay);
    }

    Chef SpawnChef()
    {
        int randomIndex = Random.Range(0,SpawnPoints.Length);
        Vector3 SpawnPos =  SpawnPoints[randomIndex].transform.position;
        return GameObject.Instantiate(ChefPrefab, SpawnPos, Quaternion.identity);
    }

}

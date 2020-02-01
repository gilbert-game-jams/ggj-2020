using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChefManager : MonoBehaviour
{
    [SerializeField]
    GameObject ChefPrefab;
    [SerializeField]
    Transform [] SpawnPoints;
    [SerializeField]
    int MaxChefs = 10;
    int NumberOfChefs = 0;
    [SerializeField]
    float MaxSpawnDelay = 20;
    [SerializeField]
    float MinSpawnDelay = 5;
    float SpawnTimer;

    private void Start() 
    {
        UpdateTimer();
    }

    private void FixedUpdate()    
    {
        if(SpawnTimer < Time.time)
        {
            if(NumberOfChefs < MaxChefs)
            {
                SpawnChef();
            }
            UpdateTimer();
        }
    }

    public void RemoveChef()
    {
        NumberOfChefs--;
    }

    void UpdateTimer()
    {
        SpawnTimer = Time.time + Random.Range(MinSpawnDelay,MaxSpawnDelay);
    }

    void SpawnChef()
    {
        int randomIndex = Random.Range(0,SpawnPoints.Length);
        Vector3 SpawnPos =  SpawnPoints[randomIndex].transform.position;
        GameObject.Instantiate(ChefPrefab, SpawnPos, Quaternion.identity);
        NumberOfChefs ++;
    }

}

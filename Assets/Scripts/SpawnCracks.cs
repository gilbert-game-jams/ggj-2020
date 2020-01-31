using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCracks : MonoBehaviour
{
    [Header ("Insertables")]
    public GameObject crackPrefab = null;
    public Renderer myRenderer = null;
    public float zOffset;

    [Header ("Changable Stats")]
    public float secondsBetweenEachCrackSpawn;
    private float crackSpawnRateCounter = 0;

    public int amountOfCracksToLose;
    private int activeCracks;

    private bool maxCracksReached;

    private void Update()
    {
        if (activeCracks >= amountOfCracksToLose)
        {
            if (!maxCracksReached)
            {
                //GAME OVER
                Debug.Log("GAME OVER");
            }
            maxCracksReached = true;

            return;
        }

        CrackTimer();
    }

    private void CrackTimer()
    {
        crackSpawnRateCounter += Time.deltaTime;

        if (crackSpawnRateCounter >= secondsBetweenEachCrackSpawn)
        {
            crackSpawnRateCounter = 0;
            
            SpawnNewCrack();
        }
    }

    private void SpawnNewCrack()
    {
        RandomizeSpawnLocation();
        CheckLocationForCracks();
    }

    public float radiusToCheckForCracks = 5f;

    private Vector3 spawnLocation;
    private void CheckLocationForCracks()
    { 
        
        bool crackHit = false;
        Collider[] hitObjects = Physics.OverlapSphere(spawnLocation, radiusToCheckForCracks);

        for (int i = 0; i < hitObjects.Length; i++)
        {
            if (hitObjects[i].CompareTag("Crack"))
            {
                crackHit = true;
            }
        }

        if (crackHit)
        {
            SpawnNewCrack();
        }
        else
        {
            InstantiateCrack();
        }
        
    }
    
    private void RandomizeSpawnLocation()
    {
        spawnLocation = new Vector3(Random.Range(-myRenderer.bounds.size.x/2, myRenderer.bounds.size.x/2), Random.Range(-myRenderer.bounds.size.y/2, myRenderer.bounds.size.y/2), zOffset);
    }

    private void InstantiateCrack()
    {
        Instantiate(crackPrefab, spawnLocation, Quaternion.identity);
        activeCracks++;
    }
}

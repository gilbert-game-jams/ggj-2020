using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCracks : MonoBehaviour
{
    [Header ("Insertables")]
    public GameObject crackPrefab = null;
    public Renderer myRenderer = null;
    public float xOffset = 3.86f;

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
        Vector3 mySpawnLocation = Vector3.zero;
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
            InstantiateCrack(mySpawnLocation);
        }
        
    }

    private void RandomizeSpawnLocation()
    {
        Debug.Log(myRenderer.bounds.size.z);
        spawnLocation = new Vector3(xOffset, Random.Range(0, myRenderer.bounds.size.y), Random.Range(-myRenderer.bounds.size.z / 2, myRenderer.bounds.size.z / 2));
    }
    private void InstantiateCrack(Vector3 mySpawnLocation)
    {

        GameObject crack = Instantiate(crackPrefab, Vector3.zero, Quaternion.identity);
        crack.transform.SetParent(transform);
        crack.transform.localPosition = spawnLocation;
        activeCracks++;
    }
}

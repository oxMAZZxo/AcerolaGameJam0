using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Bush : MonoBehaviour
{
    [SerializeField]private bool spawn;
    [SerializeField]private GameObject rabbit;
    [SerializeField,Range(1,30f)]private int spawnInterval = 1;
    [SerializeField,Range(-1, 100f),Tooltip("A value of '-1' tells the script to never stop spawning")]private int spawnCount = -1; 
    [SerializeField]private Transform spawnPosition;
    [SerializeField,Range(60,500)]private int timeUntilNextBatch = 1;
    private bool finishedSpawning;
    private bool waiting;

    void Start()
    {
        if(!spawn) {return;}
        if(spawnCount == -1)
        {
            StartCoroutine(SpawnRabbit());
            finishedSpawning = false;
        }else
        {
            finishedSpawning = false;
            StartCoroutine(SpawnRabbitWithLimit());
        }
        
    }

    void FixedUpdate()
    {
        if(finishedSpawning && !waiting)
        {
            StartCoroutine(Wait());
            waiting = true;
        }
    }

    private IEnumerator Wait()
    {
        Debug.Log("Waiting until next batch");
        yield return new WaitForSeconds(timeUntilNextBatch);
        waiting = false;
        if(spawnCount == -1)
        {
            StartCoroutine(SpawnRabbit());
            finishedSpawning = false;
        }else
        {
            finishedSpawning = false;
            StartCoroutine(SpawnRabbitWithLimit());
        }
    }

    private IEnumerator SpawnRabbit()
    {
        while(spawn)
        {
            Instantiate(rabbit,spawnPosition.position,Quaternion.identity);
            yield return new WaitForSeconds(spawnInterval);
        }
        Debug.Log("Finished spawning");
        finishedSpawning = true;
    }

    private IEnumerator SpawnRabbitWithLimit()
    {
        int counter = 0;
        while(counter < spawnCount + 1 && spawn)
        {
            Instantiate(rabbit,spawnPosition.position,Quaternion.identity);
            yield return new WaitForSeconds(spawnInterval);
            counter +=1;
        }
        Debug.Log("Finished spawning");
        finishedSpawning = true;
    }
}

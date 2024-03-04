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

    void Start()
    {
        if(!spawn) {return;}
        if(spawnCount == -1)
        {
            StartCoroutine(SpawnRabbit());
        }else
        {
            StartCoroutine(SpawnRabbitWithLimit());
        }
        
    }

    private IEnumerator SpawnRabbit()
    {
        while(spawn)
        {
            Instantiate(rabbit,transform.position,Quaternion.identity);
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private IEnumerator SpawnRabbitWithLimit()
    {
        int counter = 0;
        while(counter < spawnCount + 1)
        {
            Instantiate(rabbit,transform.position,Quaternion.identity);
            yield return new WaitForSeconds(spawnInterval);
            counter +=1;
        }
    }
}

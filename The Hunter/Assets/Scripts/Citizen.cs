using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Citizen : MonoBehaviour
{
    private Animator animator;
    [SerializeField,Range(1,10)]private int minOdds = 1;
    [SerializeField,Range(10,100)]private int maxOdds = 1;
    [SerializeField,Range(1,60)]private int waveInterval = 1;
    private float waveCounter;
    private bool isWaving;

    void Start()
    {
        animator = GetComponent<Animator>();    
    }

    
    void Update()
    {
        if(isWaving) { return;}

        if(waveCounter >= waveInterval)
        {
            if(Random.Range(minOdds,maxOdds) <= maxOdds / 10)
            {
                animator.SetTrigger("wave");
                isWaving = true;
                waveCounter = 0;
            }
        }

        waveCounter += Time.deltaTime;
    }

    public void StopWaving() {isWaving = false;}
}

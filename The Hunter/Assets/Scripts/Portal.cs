using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Portal : StaticAI
{
    [Header("Spawning")]
    [SerializeField]private bool shouldSpawn;
    [SerializeField]private Transform[] spawnTransforms;
    [SerializeField]private Troll regularTrollPrefab;
    [SerializeField]private Troll bigTrollPrefab;
    [SerializeField,Range(1,100)]private int bigTrollChance = 1;
    [SerializeField,Range(1,5)]private float spawnInterval = 1;
    [SerializeField,Range(0,0.1f)]private float spawnIntervalDecreaseRate = 0f;
    [SerializeField,Range(1,100)]private int chanceToSpawnMultiple = 1;
    [SerializeField,Range(1,100)]private int multiple = 1;
    [SerializeField,Range(-1,100)]private int chanceOfSpawningBehindPlayer = 1;
    [SerializeField,Range(1,50)]private int distanceBehindPlayer = 10;
    private bool stopSpawning;
    private bool spawning;

    void Start()
    {
        if(deathParticleSpawn == null) { deathParticleSpawn = transform;}
        state = AIState.Tracking;
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    void FixedUpdate()
    {
        switch(state)
        {
            case AIState.Tracking:
                Track();
            break;
            case AIState.Spawning:
                if(!spawning && shouldSpawn){StartCoroutine(SpawnTrolls());}
                if(Mathf.Abs(GetDistanceFromPlayer()) > triggerDistance) 
                {
                    state = AIState.Tracking;
                    spawning =  false;
                    stopSpawning = true;
                    StopAllCoroutines();
                    return;
                }
            break;
        }
    }

    protected override void Track()
    {
        float distanceFromPlayer = Mathf.Abs(GetDistanceFromPlayer());
        if(distanceFromPlayer <= triggerDistance && location == GameManager.Instance.GetPlayerLocation())
        {
            state = AIState.Spawning;
        }
    }

    private IEnumerator SpawnTrolls()
    {   
        stopSpawning = false;
        spawning = true;
        int amount;
        Troll trollToSpawn;
        float currentSpawnInterval = spawnInterval;
        while(spawning)
        {

            if(UnityEngine.Random.Range(0,101) >= bigTrollChance)
            {
                trollToSpawn = bigTrollPrefab;
            }else
            {
                trollToSpawn = regularTrollPrefab;
            }

            if(UnityEngine.Random.Range(0,101) > chanceToSpawnMultiple)
            {
                amount = multiple;
            }else
            {
                amount = 1;
            }

            Spawn(trollToSpawn,amount);
           
            yield return new WaitForSeconds(currentSpawnInterval);
            currentSpawnInterval -= spawnIntervalDecreaseRate;
        }
    }

    private void Spawn(Troll troll, int amount)
    {
        for(int i = 0; i < amount; i++)
        {
            if(stopSpawning){return;}
            
            Vector3 spawnPos = spawnTransforms[UnityEngine.Random.Range(0, spawnTransforms.Length)].position;
            if(chanceOfSpawningBehindPlayer != -1 && UnityEngine.Random.Range(0,101) > chanceOfSpawningBehindPlayer)
            {
                if(IsPlayerOnTheRight())
                {
                    spawnPos.x = PlayerCombat.Instance.transform.position.x + distanceBehindPlayer;
                }else
                {
                    spawnPos.x = PlayerCombat.Instance.transform.position.x - distanceBehindPlayer;
                }
            }
            Troll currentTroll = Instantiate(troll,spawnPos,quaternion.identity).GetComponent<Troll>();
            currentTroll.SetTriggerDistance(triggerDistance);
            currentTroll.SetLocation(location);
            
        }
        
    }

    public override void TakeDamage(float damage)
    {
        animator.SetTrigger("hurt");
        currentHealth -= damage;
        if(currentHealth < 1)
        {
            Die();
        }
    }

    protected override void Die()
    {
        StopAllCoroutines();
        GameManager.Instance.CheckPortals();
        base.Die();
    }
}

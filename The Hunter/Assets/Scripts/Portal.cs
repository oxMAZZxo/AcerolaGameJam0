using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

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
    [SerializeField]private List<Troll> trolls = new List<Troll>();
    [SerializeField,Range(1,100)]private int chanceToSpawnMultiple = 1;
    [SerializeField,Range(1,100)]private int multiple = 1;

    private bool spawning;

    void Start()
    {
        if(deathParticleSpawn == null) { deathParticleSpawn = transform;}
        state = AIState.Tracking;
        rb = GetComponent<Rigidbody2D>();
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
                if(Mathf.Abs(GetDistanceFromPlayer()) >= triggerDistance) 
                {
                    StopAllCoroutines();
                    spawning =  false;
                    return;
                }
            break;
        }
    }

    protected override void Track()
    {
        if(Mathf.Abs(GetDistanceFromPlayer()) <= triggerDistance)
        {
            state = AIState.Spawning;
        }
    }

    private IEnumerator SpawnTrolls()
    {   
        spawning = true;
        int amount;
        Troll trollToSpawn;
        float currentSpawnInterval = spawnInterval;
        while(spawning)
        {

            if(UnityEngine.Random.Range(0,100) >= bigTrollChance)
            {
                trollToSpawn = bigTrollPrefab;
            }else
            {
                trollToSpawn = regularTrollPrefab;
            }

            if(UnityEngine.Random.Range(0,100) > chanceToSpawnMultiple)
            {
                amount = multiple;
            }else
            {
                amount = 1;
            }

            Spawn(trollToSpawn,amount);
           
            yield return new WaitForSeconds(currentSpawnInterval);
            currentSpawnInterval -= spawnIntervalDecreaseRate;
            Debug.Log(currentSpawnInterval);
        }
    }

    private void Spawn(Troll troll, int amount)
    {
        for(int i = 0; i < amount; i++)
        {
            int spawnIndex = UnityEngine.Random.Range(0, spawnTransforms.Length);
            Troll currentTroll = Instantiate(troll,spawnTransforms[spawnIndex].position,quaternion.identity).GetComponent<Troll>();
            trolls.Add(currentTroll);
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
        base.Die();
    }

}

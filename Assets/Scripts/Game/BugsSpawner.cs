using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BugsSpawner : MonoBehaviour
{
    public static BugsSpawner Instance;
    public void Awake()
    {
        Instance = this;
    }

    public int BaseBugsCount = 10;
    public int MaxBugsCount = 60;
    public float BaseSpawnTime = 5f;

    public GameObject SpawnEffect;
    private float _spawnDelay = 1f;

    public int BugsLimit => Mathf.FloorToInt(BaseBugsCount + MaxBugsCount * GameRules.GetProgress());
    public float SpawnDelay => Mathf.FloorToInt(0.1f + (BaseSpawnTime - BaseSpawnTime * GameRules.GetProgress()));

    public Bug[] Bugs;
    public Transform[] SpawnPoints;
    public bool IsActive;

    public void Update()
    {
        if (!IsActive) return;

        _spawnDelay -= Time.deltaTime;
        if(_spawnDelay <= 0)
        {
            if (GameRules.BugsCount < BugsLimit)
            {
                _spawnDelay = SpawnDelay;
                Bug bug = Bug.Instantiate(Bugs[Random.Range(0, Bugs.Length)]);
                Vector2 spawnPosition = SpawnPoints[Random.Range(0, SpawnPoints.Length)].position;
                bug.transform.position = spawnPosition;

                GameObject spawnEffect = GameObject.Instantiate(SpawnEffect);
                spawnEffect.transform.position = spawnPosition;
            }
        }
    }
}

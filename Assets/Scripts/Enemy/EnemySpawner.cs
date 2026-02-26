using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Serializable]
    public class EnemySpawnEntry
    {
        public GameObject prefab;
        [Min(1)] public int cost = 1;
        [Min(1)] public int weight = 1;
    }

    [Header("Spawn Data")]
    [SerializeField] private List<EnemySpawnEntry> enemyEntries = new();

    [Header("Spawn Placement")]
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float fallbackSpawnRadius = 10f;

    [Header("Defaults")]
    [SerializeField] private float defaultSpawnInterval = 0.35f;
    [SerializeField] private Transform targetOverride;

    private readonly List<EnemyHealth> aliveEnemies = new();
    private Coroutine spawnRoutine;
    private Transform cachedPlayerTransform;

    public int AliveCount
    {
        get
        {
            CleanupDeadReferences();
            return aliveEnemies.Count;
        }
    }

    public bool IsSpawningWave { get; private set; }

    public event Action<EnemyHealth> EnemySpawned;
    public event Action<EnemyHealth> EnemyRemoved;

    void Awake()
    {
        if (targetOverride != null)
        {
            cachedPlayerTransform = targetOverride;
            return;
        }

        PlayerMovement player = FindFirstObjectByType<PlayerMovement>();
        if (player != null)
        {
            cachedPlayerTransform = player.transform;
        }
    }

    public void StartWaveSpawn(int waveBudget, float spawnInterval = -1f)
    {
        if (waveBudget <= 0)
        {
            Debug.LogWarning("EnemySpawnerPrototype.StartWaveSpawn called with non-positive budget.");
            return;
        }

        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
        }

        float interval = spawnInterval > 0f ? spawnInterval : defaultSpawnInterval;
        spawnRoutine = StartCoroutine(SpawnWaveRoutine(waveBudget, interval));
    }

    private IEnumerator SpawnWaveRoutine(int waveBudget, float spawnInterval)
    {
        IsSpawningWave = true;
        int remainingBudget = waveBudget;

        while (remainingBudget > 0)
        {
            EnemySpawnEntry entry = SelectEntryForBudget(remainingBudget);
            if (entry == null)
            {
                break;
            }

            SpawnEnemy(entry);
            remainingBudget -= Mathf.Max(1, entry.cost);

            if (spawnInterval > 0f)
            {
                yield return new WaitForSeconds(spawnInterval);
            }
            else
            {
                yield return null;
            }
        }

        IsSpawningWave = false;
        spawnRoutine = null;
    }

    private EnemySpawnEntry SelectEntryForBudget(int budget)
    {
        int totalWeight = 0;
        for (int i = 0; i < enemyEntries.Count; i++)
        {
            EnemySpawnEntry entry = enemyEntries[i];
            if (!IsEntrySpawnable(entry, budget)) continue;
            totalWeight += Mathf.Max(1, entry.weight);
        }

        if (totalWeight <= 0) return null;

        int roll = UnityEngine.Random.Range(0, totalWeight);
        for (int i = 0; i < enemyEntries.Count; i++)
        {
            EnemySpawnEntry entry = enemyEntries[i];
            if (!IsEntrySpawnable(entry, budget)) continue;

            roll -= Mathf.Max(1, entry.weight);
            if (roll < 0)
            {
                return entry;
            }
        }

        return null;
    }

    private static bool IsEntrySpawnable(EnemySpawnEntry entry, int budget)
    {
        if (entry == null || entry.prefab == null) return false;
        return entry.cost <= budget;
    }

    private void SpawnEnemy(EnemySpawnEntry entry)
    {
        Vector2 spawnPosition = GetSpawnPosition();
        GameObject enemyObject = Instantiate(entry.prefab, spawnPosition, Quaternion.identity);

        EnemyHealth health = enemyObject.GetComponentInChildren<EnemyHealth>();
        if (health != null)
        {
            aliveEnemies.Add(health);
            health.Died += OnEnemyDied;
            EnemySpawned?.Invoke(health);
        }

        EnemyFollowPrototype follower = enemyObject.GetComponentInChildren<EnemyFollowPrototype>();
        if (follower != null && cachedPlayerTransform != null)
        {
            follower.SetTarget(cachedPlayerTransform);
        }
    }

    private Vector2 GetSpawnPosition()
    {
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            Transform spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
            if (spawnPoint != null)
            {
                return spawnPoint.position;
            }
        }

        Vector2 randomDirection = UnityEngine.Random.insideUnitCircle;
        if (randomDirection.sqrMagnitude < 0.0001f)
        {
            randomDirection = Vector2.right;
        }
        randomDirection.Normalize();
        return (Vector2)transform.position + randomDirection * fallbackSpawnRadius;
    }

    private void OnEnemyDied(EnemyHealth deadEnemy)
    {
        if (deadEnemy == null) return;
        deadEnemy.Died -= OnEnemyDied;
        aliveEnemies.Remove(deadEnemy);
        EnemyRemoved?.Invoke(deadEnemy);
    }

    private void CleanupDeadReferences()
    {
        for (int i = aliveEnemies.Count - 1; i >= 0; i--)
        {
            if (aliveEnemies[i] == null)
            {
                aliveEnemies.RemoveAt(i);
            }
        }
    }

    void OnDisable()
    {
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }
        IsSpawningWave = false;

        for (int i = 0; i < aliveEnemies.Count; i++)
        {
            EnemyHealth enemy = aliveEnemies[i];
            if (enemy != null)
            {
                enemy.Died -= OnEnemyDied;
            }
        }
    }
}

using System;
using System.Collections;
using UnityEngine;

public class WaveManagerPrototype : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemySpawner enemySpawner;

    [Header("Flow")]
    [SerializeField] private bool autoStartOnPlay = true;
    [SerializeField] private int startingWave = 1;
    [SerializeField] private float timeBetweenWaves = 2f;

    [Header("Difficulty Scaling")]
    [SerializeField] private int baseWaveBudget = 6;
    [SerializeField] private int budgetIncreasePerWave = 3;
    [SerializeField] private float baseSpawnInterval = 0.45f;
    [SerializeField] private float spawnIntervalMultiplierPerWave = 0.92f;
    [SerializeField] private float minimumSpawnInterval = 0.1f;

    private Coroutine waveLoopRoutine;

    public int CurrentWave { get; private set; }
    public bool IsWaveActive { get; private set; }

    public event Action<int> WaveStarted;
    public event Action<int> WaveCleared;

    private bool waitingForIntermission;
    private bool intermissionComplete;

    void Start()
    {
        if (!autoStartOnPlay) return;
        StartWaveLoop();
    }

    public void StartWaveLoop()
    {
        if (waveLoopRoutine != null) return;
        if (enemySpawner == null)
        {
            Debug.LogError("WaveManagerPrototype requires an EnemySpawnerPrototype reference.");
            return;
        }

        CurrentWave = Mathf.Max(1, startingWave) - 1;
        waveLoopRoutine = StartCoroutine(WaveLoopRoutine());
    }

    public void StopWaveLoop()
    {
        if (waveLoopRoutine == null) return;
        StopCoroutine(waveLoopRoutine);
        waveLoopRoutine = null;
        IsWaveActive = false;
    }

    private IEnumerator WaveLoopRoutine()
    {
        while (true)
        {
            CurrentWave++;
            yield return RunSingleWave(CurrentWave);

            waitingForIntermission = true;
            intermissionComplete = false;
            WaveCleared?.Invoke(CurrentWave);

            yield return new WaitUntil(() => intermissionComplete);

            if (timeBetweenWaves > 0f)
            {
                yield return new WaitForSeconds(timeBetweenWaves);
            }
            else
            {
                yield return null;
            }
        }
    }

    private IEnumerator RunSingleWave(int waveNumber)
    {
        IsWaveActive = true;
        WaveStarted?.Invoke(waveNumber);

        int budget = GetWaveBudget(waveNumber);
        float spawnInterval = GetSpawnInterval(waveNumber);
        enemySpawner.StartWaveSpawn(budget, spawnInterval);

        while (enemySpawner.IsSpawningWave || enemySpawner.AliveCount > 0)
        {
            yield return null;
        }

        IsWaveActive = false;
    }

    protected virtual int GetWaveBudget(int waveNumber)
    {
        int waveIndex = Mathf.Max(0, waveNumber - 1);
        return Mathf.Max(1, baseWaveBudget + waveIndex * budgetIncreasePerWave);
    }

    protected virtual float GetSpawnInterval(int waveNumber)
    {
        int waveIndex = Mathf.Max(0, waveNumber - 1);
        float scaled = baseSpawnInterval * Mathf.Pow(spawnIntervalMultiplierPerWave, waveIndex);
        return Mathf.Max(minimumSpawnInterval, scaled);
    }

    public void CompleteIntermission()
    {
        if (!waitingForIntermission) return;
        intermissionComplete = true;
    }
}

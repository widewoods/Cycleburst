using System;
using System.Collections;
using UnityEngine;

public class IntermissionManager : MonoBehaviour
{
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private WaveManagerPrototype waveManagerPrototype;

    public event Action intermissionStarted;
    public event Action intermissionEnded;

    void OnEnable()
    {
        waveManagerPrototype.WaveCleared += HandleWaveCleared;
    }

    void OnDisable()
    {
        waveManagerPrototype.WaveCleared -= HandleWaveCleared;
    }

    private void HandleWaveCleared(int clearedWaveNumber)
    {
        shopPanel.SetActive(true);
        intermissionStarted?.Invoke();
    }

    public void FinishIntermission()
    {
        shopPanel.SetActive(false);
        waveManagerPrototype.CompleteIntermission();
        intermissionEnded?.Invoke();
    }
}

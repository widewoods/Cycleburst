using System.Collections;
using UnityEngine;

public class IntermissionManager : MonoBehaviour
{
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private WaveManagerPrototype waveManagerPrototype;

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
    }

    public void FinishIntermission()
    {
        shopPanel.SetActive(false);
        waveManagerPrototype.CompleteIntermission();
    }
}

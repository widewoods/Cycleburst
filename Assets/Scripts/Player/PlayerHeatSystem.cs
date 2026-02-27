using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerHeatSystem : MonoBehaviour
{
    [SerializeField] private int maxHeat;
    [SerializeField] private int passiveDecayPerSec;
    [SerializeField] private int overheatedDecayPerSec;

    private int currentHeat;
    private bool overheated;

    public int MaxHeat => maxHeat;
    public bool Overheated => overheated;

    public event Action<int> OnHeatChange;

    private Coroutine passiveHeatDecay;

    void Awake()
    {
        currentHeat = 0;
        OnHeatChange?.Invoke(currentHeat);
        overheated = false;
    }

    float timer = 0f;

    void Update()
    {
        if (currentHeat <= 0) return;
        timer += Time.deltaTime;
        if (timer >= 1f)
        {
            int decayAmount = overheated ? overheatedDecayPerSec : passiveDecayPerSec;
            ChangeHeat(-decayAmount);
            timer = 0f;
        }
    }

    public void ChangeHeat(int amount)
    {
        currentHeat += amount;
        if (currentHeat <= 0)
        {
            overheated = false;
        }
        else if (currentHeat >= maxHeat)
        {
            overheated = true;
        }

        currentHeat = Mathf.Clamp(currentHeat, 0, maxHeat);

        OnHeatChange?.Invoke(currentHeat);
    }

}

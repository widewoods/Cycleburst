using System;
using UnityEngine;

public class EnemyRulePulseBehaviour : EnemyRuleBehaviour
{
    [SerializeField] private float pulseInterval = 4f;

    private float pulseTimer;
    public event Action PulseTriggered;

    protected override void OnInitialized()
    {
        pulseTimer = pulseInterval;
    }

    public override void TickRule(float deltaTime)
    {
        if (pulseInterval <= 0f) return;

        pulseTimer -= deltaTime;
        if (pulseTimer > 0f) return;

        pulseTimer += pulseInterval;
        PulseTriggered?.Invoke();
        OnPulse();
    }

    protected virtual void OnPulse()
    {
    }
}

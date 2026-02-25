using UnityEngine;

public struct DamageRequest
{
    public Transform Source;
    public float Amount;
    // public DamageType Type; Avoid scope creep
}

public interface IDamageable
{
    float CurrentHealth { get; }
    float MaxHealth { get; }
    bool IsAlive { get; }
    void TakeDamage(in DamageRequest request);
}

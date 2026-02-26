using System;
using UnityEngine;

public class EnemyHealthPrototype : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 10;
    private float currentHealth;
    private bool hasDied;

    public float CurrentHealth => currentHealth;

    public float MaxHealth => maxHealth;

    public bool IsAlive => currentHealth > 0;
    public event Action<EnemyHealthPrototype> Died;

    void Awake()
    {
        currentHealth = maxHealth;
        hasDied = false;
    }

    public void TakeDamage(in DamageRequest request)
    {
        if (hasDied) return;

        currentHealth -= request.Amount;
        if (!IsAlive)
        {
            HandleDeath();
        }
    }

    private void HandleDeath()
    {
        if (hasDied) return;

        hasDied = true;
        Died?.Invoke(this);
        Destroy(gameObject);
    }
}

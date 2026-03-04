using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 10;
    private float currentHealth;
    private bool hasDied;

    public float CurrentHealth => currentHealth;

    public float MaxHealth => maxHealth;

    public bool IsAlive => currentHealth > 0;
    public event Action<EnemyHealth> OnDeath;

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
        OnDeath?.Invoke(this);
        Destroy(gameObject);
    }
}

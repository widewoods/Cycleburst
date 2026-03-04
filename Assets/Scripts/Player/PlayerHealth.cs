using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    private float currentHealth;
    [SerializeField] private float maxHealth;

    public float CurrentHealth => currentHealth;

    public float MaxHealth => maxHealth;

    public bool IsAlive => currentHealth > 0;

    public event Action OnPlayerDeath;
    public event Action<float> OnPlayerHit;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(in DamageRequest request)
    {
        currentHealth -= request.Amount;
        Debug.Log($"Current player health: {currentHealth}");

        if (!IsAlive)
        {
            HandleDeath();
        }
        else
        {
            OnPlayerHit?.Invoke(request.Amount);
        }
    }

    private void HandleDeath()
    {
        Debug.Log("Game over");
        OnPlayerDeath?.Invoke();
        Destroy(gameObject);
    }
}

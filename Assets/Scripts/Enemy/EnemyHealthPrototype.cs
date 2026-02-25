using UnityEngine;

public class EnemyHealthPrototype : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 10;
    private float currentHealth;

    public float CurrentHealth => currentHealth;

    public float MaxHealth => maxHealth;

    public bool IsAlive => currentHealth > 0;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(in DamageRequest request)
    {
        currentHealth -= request.Amount;
        if (!IsAlive)
        {
            HandleDeath();
        }
    }

    private void HandleDeath()
    {
        Destroy(gameObject);
    }
}

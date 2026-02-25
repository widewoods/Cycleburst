using UnityEngine;

public class DamageService : MonoBehaviour
{
    public bool TryDeal(Transform source, Component target, float amount)
    {
        if (!target.TryGetComponent<IDamageable>(out var damageable)) return false;
        if (!damageable.IsAlive) return false;

        var request = new DamageRequest
        {
            Source = source,
            Amount = amount,
        };

        damageable.TakeDamage(in request);
        return true;
    }
}

using UnityEngine;

public class KnockbackHitbox : AttackHitboxBase
{
    private float knockbackDistance;
    private float life;
    public void Initialize(WorldServices worldServices, Transform damageSource, float knockback, float lifetime = 0.2f)
    {
        knockbackDistance = knockback;
        life = lifetime;
        InitializeBase(worldServices, damageSource, lifetime);
    }

    protected override void OnTargetResolved(Collider2D collider, Component damageTarget)
    {
        Vector2 dir = ((Vector2)damageTarget.transform.position - (Vector2)Source.position).normalized;
        if (dir.sqrMagnitude < 0.0001f) dir = Vector2.right;

        IKnockbackReceiver knockbackReceiver = damageTarget.GetComponent<IKnockbackReceiver>();
        if (knockbackReceiver == null)
        {
            knockbackReceiver = damageTarget.GetComponentInParent<IKnockbackReceiver>();
        }
        if (knockbackReceiver == null) return;

        knockbackReceiver.ApplyKnockback(dir, knockbackDistance, life);
    }
}

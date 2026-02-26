using UnityEngine;

public class AttackHitbox : AttackHitboxBase
{
    private float damageAmount;

    public void Initialize(WorldServices worldServices, Transform damageSource, float dmg, float lifetime = 0.2f)
    {
        damageAmount = dmg;
        InitializeBase(worldServices, damageSource, lifetime);
    }

    protected override void OnTargetResolved(Collider2D collider, Component damageTarget)
    {
        World.Damage.TryDeal(Source, damageTarget, damageAmount);
    }
}

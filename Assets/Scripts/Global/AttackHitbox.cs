using System.Collections.Generic;
using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    private WorldServices world;
    private Transform source;
    private float damageAmount;
    private readonly HashSet<int> hitTargets = new();


    public void Initialize(WorldServices worldServices, Transform damageSource, float dmg, float lifetime = 0.2f)
    {
        world = worldServices;
        source = damageSource;
        damageAmount = dmg;
        hitTargets.Clear();
        Invoke(nameof(RemoveSelf), lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (world == null || other == null) return;
        if (IsSourceCollider(other)) return;

        Component damageTarget = world.Damage.ResolveDamageTarget(other);
        if (damageTarget == null) return;

        int targetId = damageTarget.GetInstanceID();
        if (!hitTargets.Add(targetId)) return;

        world.Damage.TryDeal(source, damageTarget, damageAmount);
    }

    private void RemoveSelf()
    {
        Destroy(gameObject);
    }

    private bool IsSourceCollider(Collider2D other)
    {
        if (source == null) return false;
        return other.transform.root == source.root;
    }
}

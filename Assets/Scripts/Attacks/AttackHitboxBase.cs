using System.Collections.Generic;
using UnityEngine;

public abstract class AttackHitboxBase : MonoBehaviour
{
    private readonly HashSet<int> hitTargets = new();
    private readonly List<Collider2D> hitColliders = new();

    protected WorldServices World { get; private set; }
    protected Transform Source { get; private set; }

    public IReadOnlyList<Collider2D> HitColliders => hitColliders;

    protected void InitializeBase(WorldServices worldServices, Transform damageSource, float lifetime = 0.2f)
    {
        World = worldServices;
        Source = damageSource;

        hitTargets.Clear();
        hitColliders.Clear();

        CancelInvoke(nameof(RemoveSelf));
        if (lifetime > 0f)
        {
            Invoke(nameof(RemoveSelf), lifetime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (World == null || other == null) return;
        if (IsSourceCollider(other)) return;

        Component damageTarget = World.Damage.ResolveDamageTarget(other);
        if (damageTarget == null) return;

        int targetId = damageTarget.GetInstanceID();
        if (!hitTargets.Add(targetId)) return;

        hitColliders.Add(other);
        OnTargetResolved(other, damageTarget);
    }

    protected abstract void OnTargetResolved(Collider2D collider, Component damageTarget);

    protected virtual void RemoveSelf()
    {
        Destroy(gameObject);
    }

    protected bool IsSourceCollider(Collider2D other)
    {
        if (Source == null) return false;
        return other.transform.root == Source.root;
    }
}

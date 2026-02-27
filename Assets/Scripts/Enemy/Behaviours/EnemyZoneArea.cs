using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EnemyZoneArea : MonoBehaviour
{
    [SerializeField] private bool affectOnlyPlayer = true;

    private readonly Dictionary<int, float> nextTickByTarget = new();

    private WorldServices world;
    private Transform source;
    private float damagePerTick;
    private float tickInterval;

    public void Initialize(WorldServices worldServices, Transform damageSource, float damage, float interval, float lifetime)
    {
        world = worldServices;
        source = damageSource;
        damagePerTick = damage;
        tickInterval = Mathf.Max(0.05f, interval);
        nextTickByTarget.Clear();

        CancelInvoke(nameof(RemoveSelf));
        if (lifetime > 0f)
        {
            Invoke(nameof(RemoveSelf), lifetime);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (world == null || other == null) return;
        if (source != null && other.transform.root == source.root) return;

        Component damageTarget = world.Damage.ResolveDamageTarget(other);
        if (damageTarget == null) return;
        if (affectOnlyPlayer && !damageTarget.transform.root.CompareTag("Player")) return;

        int targetId = damageTarget.GetInstanceID();
        float now = Time.time;
        if (nextTickByTarget.TryGetValue(targetId, out float nextTickTime) && now < nextTickTime)
        {
            return;
        }

        nextTickByTarget[targetId] = now + tickInterval;
        world.Damage.TryDeal(source, damageTarget, damagePerTick);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other == null || world == null) return;
        Component damageTarget = world.Damage.ResolveDamageTarget(other);
        if (damageTarget == null) return;
        nextTickByTarget.Remove(damageTarget.GetInstanceID());
    }

    private void RemoveSelf()
    {
        Destroy(gameObject);
    }
}

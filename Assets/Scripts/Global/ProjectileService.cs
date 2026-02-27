using UnityEngine;

public class ProjectileService : MonoBehaviour
{
    public void SpawnProjectile(
    WorldServices world,
    Transform source,
    Vector2 spawnPosition,
    Vector2 direction,
    GameObject prefab,
    float damage,
    float speed)
    {
        if (prefab == null) return;
        if (direction.sqrMagnitude < 0.0001f) direction = Vector2.up;
        direction.Normalize();

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        GameObject obj = Instantiate(prefab, spawnPosition, Quaternion.Euler(0f, 0f, angle));

        if (obj.TryGetComponent<IProjectile>(out var projectile))
        {
            projectile.Initialize(world, source, damage, speed);
        }
    }

    public void SpawnProjectile(CardContext ctx, GameObject prefab, float damage, float speed)
    {
        Vector2 pos = (Vector2)ctx.caster.position + ctx.aimDir.normalized;
        SpawnProjectile(ctx.world, ctx.caster, pos, ctx.aimDir, prefab, damage, speed);
    }

}

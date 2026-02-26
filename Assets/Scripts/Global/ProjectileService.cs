using UnityEngine;

public class ProjectileService : MonoBehaviour
{
    public void SpawnProjectile(CardContext ctx, GameObject prefab, float damage, float speed)
    {
        Vector2 spawnPosition = (Vector2)ctx.caster.position + ctx.aimDir.normalized;

        float spawnAngle = Mathf.Atan2(ctx.aimDir.y, ctx.aimDir.x) * Mathf.Rad2Deg - 90f;

        GameObject obj = Instantiate(prefab, spawnPosition, Quaternion.Euler(0, 0, spawnAngle));
        obj.GetComponent<IProjectile>().Initialize(ctx.world, ctx.caster, damage, speed);
    }
}

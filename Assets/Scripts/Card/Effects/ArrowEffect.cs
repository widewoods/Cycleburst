using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Effects/Arrow")]
public class ArrowEffect : CardEffect
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float damage;
    [SerializeField] private float speed;

    public override void Resolve(CardContext ctx)
    {
        ctx.world.Projectile.SpawnProjectile(ctx, projectilePrefab, damage, speed);
    }
}

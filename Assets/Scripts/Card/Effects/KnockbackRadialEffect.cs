using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Effects/KnockbackRadial")]
public class KnockbackRadialEffect : CardEffect
{
    [SerializeField] private GameObject hitboxPrefab;
    [SerializeField] private float knockbackDistance;

    public override void Resolve(CardContext ctx)
    {
        Vector2 hitboxSpawnPosition = ctx.caster.position;

        GameObject obj = Instantiate(hitboxPrefab, hitboxSpawnPosition, Quaternion.identity);

        var hitbox = obj.GetComponent<KnockbackHitbox>();
        hitbox.Initialize(ctx.world, ctx.caster, knockbackDistance);
    }
}

using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Effects/Slash")]
public class SlashEffect : CardEffect
{
    [SerializeField] private GameObject hitboxPrefab;
    [SerializeField] private float damage;

    public override void Resolve(CardContext ctx)
    {
        float hitboxHeight = hitboxPrefab.GetComponent<BoxCollider2D>().size.y;
        Vector2 hitboxSpawnPosition = (Vector2)ctx.caster.position + ctx.aimDir.normalized * hitboxHeight;

        float spawnAngle = Mathf.Atan2(ctx.aimDir.y, ctx.aimDir.x) * Mathf.Rad2Deg - 90f;

        GameObject obj = Instantiate(hitboxPrefab, hitboxSpawnPosition, Quaternion.Euler(0f, 0f, spawnAngle));


        var hitbox = obj.GetComponent<AttackHitbox>();
        hitbox.Initialize(ctx.world, ctx.caster, damage);
    }
}

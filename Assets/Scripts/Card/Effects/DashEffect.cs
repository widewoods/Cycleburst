using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Effects/Dash")]
public class DashEffect : CardEffect
{
    [SerializeField] private float distance = 4f;
    [SerializeField] private float duration = 0.2f;


    public override void Resolve(CardContext ctx)
    {
        ctx.world.Movement.Dash(ctx.caster, ctx.aimDir, distance, duration);
    }

    public override IEnumerator ResolveSequence(CardContext ctx)
    {
        ctx.world.Movement.Dash(ctx.caster, ctx.aimDir, distance, duration);
        yield return new WaitForSeconds(duration);
    }

}
using UnityEngine;

public class EnemyCollisionAttackBehavior : EnemyAttackBehavior
{
    [SerializeField] private float damage;

    public override void TickAttack(float deltaTime)
    {

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (Context == null) { Debug.LogWarning("Enemy Missing Context"); return; }
        if (Context.World == null) return;
        if (collision == null) return;


        Component damageTarget = Context.World.Damage.ResolveDamageTarget(collision.collider);
        if (damageTarget == null) return;
        if (!damageTarget.transform.root.CompareTag("Player")) return;

        Context.World.Damage.TryDeal(transform, damageTarget, damage);
    }
}

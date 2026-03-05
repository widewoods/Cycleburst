using UnityEngine;

public class EnemyCollisionAttackBehavior : EnemyAttackBehavior
{
    [SerializeField] private float damage;
    [SerializeField] private float attackCooldown = 1f;

    private float attackTimer = 0f;
    private bool CanAttack => attackTimer >= attackCooldown;

    public override void TickAttack(float deltaTime)
    {
        attackTimer += deltaTime;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (Context == null) { Debug.LogWarning("Enemy Missing Context"); return; }
        if (Context.World == null) return;
        if (collision == null) return;

        if (!CanAttack) return;


        Component damageTarget = Context.World.Damage.ResolveDamageTarget(collision.collider);
        if (damageTarget == null) return;
        if (!damageTarget.transform.root.CompareTag("Player")) return;

        Context.World.Damage.TryDeal(transform, damageTarget, damage);
        attackTimer = 0f;
    }
}

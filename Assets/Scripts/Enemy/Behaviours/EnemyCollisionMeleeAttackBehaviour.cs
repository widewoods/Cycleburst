using UnityEngine;

public class EnemyCollisionMeleeAttackBehaviour : EnemyAttackBehaviour
{
    [SerializeField] private float damage = 3f;
    [SerializeField] private float attackCooldown = 0.75f;
    [SerializeField] private bool requirePlayerTag = true;

    private float cooldownRemaining;

    public override void TickAttack(float deltaTime)
    {
        cooldownRemaining -= deltaTime;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryAttackCollision(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        TryAttackCollision(collision);
    }

    private void TryAttackCollision(Collision2D collision)
    {
        if (!IsInitialized || Context == null || Context.World == null) return;
        if (collision == null || cooldownRemaining > 0f) return;

        Component damageTarget = ResolveDamageTarget(collision.collider, collision.otherCollider);
        if (damageTarget == null) return;
        if (requirePlayerTag && !damageTarget.transform.root.CompareTag("Player")) return;

        bool dealt = Context.World.Damage.TryDeal(Context.Self, damageTarget, damage);
        if (dealt)
        {
            cooldownRemaining = attackCooldown;
        }
    }

    private Component ResolveDamageTarget(Collider2D first, Collider2D second)
    {
        Component firstTarget = ResolveDamageTarget(first);
        if (firstTarget != null) return firstTarget;
        return ResolveDamageTarget(second);
    }

    private Component ResolveDamageTarget(Collider2D collider)
    {
        if (collider == null) return null;
        return Context.World.Damage.ResolveDamageTarget(collider);
    }
}

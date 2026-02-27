using System.Collections;
using UnityEngine;

public class EnemyBulletShooterBehavior : EnemyAttackBehavior
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletDamage;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float attackCooldown;
    [SerializeField] private float attackWindup;

    private float attackTimer;
    private bool isCasting;
    private Coroutine shootRoutine;

    void Awake()
    {
        attackTimer = 0f;
    }

    public override void TickAttack(float deltaTime)
    {
        if (isCasting) return;
        if (Context == null || Context.World == null || Context.World.Projectile == null) return;
        if (Context.Target == null || bulletPrefab == null) return;

        attackTimer += deltaTime;
        if (attackTimer >= attackCooldown)
        {
            attackTimer = 0f;
            shootRoutine = StartCoroutine(ShootProjectileRoutine());
        }
    }

    private IEnumerator ShootProjectileRoutine()
    {
        if (Context == null)
        {
            Debug.LogWarning("Enemy context missing");
            yield break;
        }

        isCasting = true;
        Context.State.LockMovement = true;

        if (attackWindup > 0f)
        {
            yield return new WaitForSeconds(attackWindup);
        }

        if (Context.Target != null && Context.World != null && Context.World.Projectile != null)
        {
            Vector2 dir = Context.Target.position - transform.position;

            Context.World.Projectile.SpawnProjectile(
                Context.World, transform, transform.position,
                dir, bulletPrefab, bulletDamage, bulletSpeed);
        }

        Context.State.LockMovement = false;
        isCasting = false;
        shootRoutine = null;
    }

    protected override void OnTerminate()
    {
        if (shootRoutine != null)
        {
            StopCoroutine(shootRoutine);
            shootRoutine = null;
        }
        isCasting = false;
        if (Context != null)
        {
            Context.State.LockMovement = false;
        }
    }
}

using UnityEngine;

public class SimpleProjectile : MonoBehaviour
{
    private Vector2 moveDirection;
    private Rigidbody2D rb;
    private WorldServices world;
    private Transform source;
    private float damageAmount;


    public void Initialize(WorldServices worldServices, Transform damageSource, float dmg, float speed, float lifetime = 4f)
    {
        rb = GetComponent<Rigidbody2D>();
        world = worldServices;
        moveDirection = transform.up;
        source = damageSource;
        damageAmount = dmg;

        rb.linearVelocity = moveDirection * speed;

        Invoke(nameof(RemoveSelf), lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (world == null || other == null) return;
        if (IsSourceCollider(other)) return;

        Component damageTarget = ResolveDamageTarget(other);
        if (damageTarget == null) return;

        if (world.Damage.TryDeal(source, damageTarget, damageAmount))
        {
            Destroy(gameObject);
        }
    }

    private void RemoveSelf()
    {
        Destroy(gameObject);
    }

    private bool IsSourceCollider(Collider2D other)
    {
        if (source == null) return false;
        return other.transform.root == source.root;
    }

    private static Component ResolveDamageTarget(Collider2D other)
    {
        if (other.TryGetComponent<IDamageable>(out _))
        {
            return other;
        }

        IDamageable damageableInParent = other.GetComponentInParent<IDamageable>();
        return damageableInParent as Component;
    }
}

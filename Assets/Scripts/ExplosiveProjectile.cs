using UnityEngine;

public class ExplosiveProjectile : MonoBehaviour, IProjectile
{
    [SerializeField] private GameObject explosionHitbox;

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

        Invoke(nameof(Explode), lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (world == null || other == null) return;
        if (IsSourceCollider(other)) return;

        Component damageTarget = world.Damage.ResolveDamageTarget(other);
        if (damageTarget == null) return;

        Explode();
    }

    private void Explode()
    {
        GameObject obj = Instantiate(explosionHitbox, transform.position, Quaternion.identity);

        var hitbox = obj.GetComponent<AttackHitbox>();
        hitbox.Initialize(world, source, damageAmount);

        Destroy(gameObject);
    }

    private bool IsSourceCollider(Collider2D other)
    {
        if (source == null) return false;
        return other.transform.root == source.root;
    }
}

using UnityEngine;

public class EnemyAttackCollider : MonoBehaviour
{
    private WorldServices world;
    [SerializeField] private float damage;

    void Awake()
    {
        Initialize();
    }

    //Temporary
    public void Initialize()
    {
        world = FindFirstObjectByType<WorldServices>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (world == null) return;
        if (collision == null) return;


        Component damageTarget = world.Damage.ResolveDamageTarget(collision.collider);
        if (damageTarget == null) return;
        if (!damageTarget.transform.root.CompareTag("Player")) return;

        world.Damage.TryDeal(transform, damageTarget, damage);
    }
}

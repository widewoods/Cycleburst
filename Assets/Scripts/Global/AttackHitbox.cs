using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    private WorldServices world;
    private Transform source;
    private float amount;


    public void Initialize(WorldServices worldServices, Transform damageSource, float damageAmount)
    {
        world = worldServices;
        source = damageSource;
        amount = damageAmount;
        Invoke(nameof(RemoveSelf), 0.2f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (world == null) return;
        world.Damage.TryDeal(source, other, amount);
    }

    private void RemoveSelf()
    {
        Destroy(gameObject);
    }
}

using UnityEngine;

public class EnemyZoneControlAttackBehaviour : EnemyAttackBehaviour
{
    [Header("Zone Spawn")]
    [SerializeField] private GameObject zonePrefab;
    [SerializeField] private float castCooldown = 3f;
    [SerializeField] private bool spawnAtTarget = true;
    [SerializeField] private Vector2 spawnOffset;

    [Header("Zone Damage")]
    [SerializeField] private float zoneLifetime = 2.5f;
    [SerializeField] private float damagePerTick = 1f;
    [SerializeField] private float tickInterval = 0.5f;

    private float cooldownRemaining;

    public override void TickAttack(float deltaTime)
    {
        if (Context == null || !Context.HasValidTarget) return;
        if (zonePrefab == null) return;

        cooldownRemaining -= deltaTime;
        if (cooldownRemaining > 0f) return;

        SpawnZone();
        cooldownRemaining = castCooldown;
    }

    private void SpawnZone()
    {
        Vector2 basePosition = spawnAtTarget ? (Vector2)Context.Target.position : (Vector2)Context.Self.position;
        Vector2 spawnPosition = basePosition + spawnOffset;

        GameObject zoneObject = Instantiate(zonePrefab, spawnPosition, Quaternion.identity);

        EnemyZoneArea zoneArea = zoneObject.GetComponent<EnemyZoneArea>();
        if (zoneArea != null)
        {
            zoneArea.Initialize(Context.World, Context.Self, damagePerTick, tickInterval, zoneLifetime);
            return;
        }

        if (zoneLifetime > 0f)
        {
            Destroy(zoneObject, zoneLifetime);
        }
    }
}

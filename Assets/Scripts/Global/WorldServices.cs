using UnityEngine;

public class WorldServices : MonoBehaviour
{
    [SerializeField] private MovementService movement;
    [SerializeField] private DamageService damage;
    [SerializeField] private ProjectileService projectile;

    [SerializeField] private WaveManagerPrototype waveManager;

    public MovementService Movement => movement;
    public DamageService Damage => damage;
    public ProjectileService Projectile => projectile;
    public WaveManagerPrototype Wave => waveManager;
}

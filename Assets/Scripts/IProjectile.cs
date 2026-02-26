using UnityEngine;

public interface IProjectile
{
    void Initialize(WorldServices worldServices, Transform damageSource, float dmg, float speed, float lifetime = 3f);
}

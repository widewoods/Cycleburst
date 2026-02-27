using UnityEngine;

public abstract class EnemyMovementBehavior : EnemyBehaviorBase
{
    public abstract void TickMovement(float deltaTime);
}

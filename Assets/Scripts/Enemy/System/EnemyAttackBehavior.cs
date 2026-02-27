using UnityEngine;

public abstract class EnemyAttackBehavior : EnemyBehaviorBase
{
    public abstract void TickAttack(float deltaTime);
}

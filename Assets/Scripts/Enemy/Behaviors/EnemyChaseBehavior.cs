using UnityEngine;

public class EnemyChaseBehavior : EnemyMovementBehavior
{
    [SerializeField] private float moveSpeed = 2.5f;

    public override void TickMovement(float deltaTime)
    {
        if (Context == null) Debug.LogWarning($"Chase context of enemy is null");
        if (Context.Target == null || Context.Body == null) return;

        Vector2 current = Context.Body.position;
        Vector2 destination = Context.Target.position;
        Vector2 dir = destination - current;
        dir.Normalize();

        Context.Body.linearVelocity = dir * moveSpeed;
    }

    private void StopMotion()
    {
        if (Context?.Body != null)
        {
            Context.Body.linearVelocity = Vector2.zero;
        }
    }
}

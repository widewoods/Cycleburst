using UnityEngine;

public class EnemyChaseBehavior : EnemyMovementBehavior
{
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float stopDistance = 0f;

    public override void TickMovement(float deltaTime)
    {
        if (Context == null) Debug.LogWarning($"Chase context of enemy is null");
        if (Context.Target == null || Context.Body == null) return;

        if (Context.State.LockMovement)
        {
            StopMotion();
            return;
        }

        Vector2 current = Context.Body.position;
        Vector2 destination = Context.Target.position;
        Vector2 dir = destination - current;
        float distance = dir.magnitude;
        dir.Normalize();

        Vector3 directionToTarget = Context.Target.position - transform.position;
        directionToTarget.Normalize();

        float angleRadians = Mathf.Atan2(directionToTarget.y, directionToTarget.x);
        float angleDegrees = angleRadians * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angleDegrees);

        if (distance > stopDistance)
        {
            Context.Body.linearVelocity = dir * moveSpeed;
        }
        else
        {
            StopMotion();
        }

    }

    private void StopMotion()
    {
        Context.Body.linearVelocity = Vector2.zero;
    }
}

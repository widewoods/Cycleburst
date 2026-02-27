using UnityEngine;

public class EnemyChaseMovementBehaviour : EnemyMovementBehaviour
{
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float stoppingDistance = 0.05f;

    public override void TickMovement(float deltaTime)
    {
        if (Context == null || !Context.HasValidTarget)
        {
            StopMotion();
            return;
        }

        Vector2 currentPosition = Context.Body != null ? Context.Body.position : (Vector2)Context.Self.position;
        Vector2 targetPosition = Context.Target.position;
        Vector2 toTarget = targetPosition - currentPosition;

        float distance = toTarget.magnitude;
        if (distance <= stoppingDistance)
        {
            StopMotion();
            return;
        }

        Vector2 direction = toTarget / Mathf.Max(0.0001f, distance);
        if (Context.Body != null)
        {
            Context.Body.linearVelocity = direction * moveSpeed;
        }
        else
        {
            Context.Self.position = Vector2.MoveTowards(currentPosition, targetPosition, moveSpeed * deltaTime);
        }
    }

    protected override void OnShutdown()
    {
        StopMotion();
    }

    private void StopMotion()
    {
        if (Context?.Body != null)
        {
            Context.Body.linearVelocity = Vector2.zero;
        }
    }
}

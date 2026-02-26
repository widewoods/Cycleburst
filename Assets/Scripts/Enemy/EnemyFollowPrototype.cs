using System.Collections;
using UnityEngine;

public class EnemyFollowPrototype : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float moveSpeed = 2.5f;

    private Rigidbody2D rb;
    private bool stunned = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        if (target == null)
        {
            PlayerMovement player = FindFirstObjectByType<PlayerMovement>();
            if (player != null)
            {
                target = player.transform;
            }
        }
    }

    void FixedUpdate()
    {
        if (target == null) return;
        if (stunned) return;

        Vector2 current = rb != null ? rb.position : (Vector2)transform.position;
        Vector2 destination = target.position;
        Vector2 dir = destination - current;
        dir.Normalize();

        if (rb != null)
        {
            rb.linearVelocity = dir * moveSpeed;
        }
    }

    public void SetTarget(Transform targetTransform)
    {
        target = targetTransform;
    }

    private IEnumerator KnockbackRoutine(Vector2 direction, float distance, float duration)
    {
        stunned = true;

        float speed = distance / Mathf.Max(0.01f, duration);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            rb.linearVelocity = direction * speed;
            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        rb.linearVelocity = Vector2.zero;
        stunned = false;
    }


    public void Knockback(Vector2 direction, float distance, float duration)
    {
        direction = direction.normalized;
        StartCoroutine(KnockbackRoutine(direction, distance, duration));
    }
}

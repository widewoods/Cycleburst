using System.Collections;
using UnityEngine;

public class EnemyFollowPrototype : MonoBehaviour, IKnockbackReceiver
{
    [SerializeField] private Transform target;
    [SerializeField] private float moveSpeed = 2.5f;

    private Rigidbody2D rb;
    private bool stunned = false;
    private Coroutine knockbackRoutine;

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
        else
        {
            transform.position = Vector2.MoveTowards(current, destination, moveSpeed * Time.fixedDeltaTime);
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
            if (rb != null)
            {
                rb.linearVelocity = direction * speed;
            }
            else
            {
                transform.position += (Vector3)(direction * speed * Time.fixedDeltaTime);
            }
            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
        stunned = false;
        knockbackRoutine = null;
    }


    public void ApplyKnockback(Vector2 direction, float distance, float duration)
    {
        direction = direction.normalized;
        if (knockbackRoutine != null)
        {
            StopCoroutine(knockbackRoutine);
        }
        knockbackRoutine = StartCoroutine(KnockbackRoutine(direction, distance, duration));
    }

    void OnDisable()
    {
        if (knockbackRoutine != null)
        {
            StopCoroutine(knockbackRoutine);
            knockbackRoutine = null;
        }
        stunned = false;
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
}

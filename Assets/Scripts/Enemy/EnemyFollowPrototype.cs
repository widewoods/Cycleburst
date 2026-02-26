using UnityEngine;

public class EnemyFollowPrototype : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float moveSpeed = 2.5f;

    private Rigidbody2D rb;

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

        Vector2 current = rb != null ? rb.position : (Vector2)transform.position;
        Vector2 destination = target.position;
        Vector2 next = Vector2.MoveTowards(current, destination, moveSpeed * Time.fixedDeltaTime);

        if (rb != null)
        {
            rb.MovePosition(next);
        }
        else
        {
            transform.position = next;
        }
    }

    public void SetTarget(Transform targetTransform)
    {
        target = targetTransform;
    }
}

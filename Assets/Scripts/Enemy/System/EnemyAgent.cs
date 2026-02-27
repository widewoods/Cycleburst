using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class EnemyAgent : MonoBehaviour, IKnockbackReceiver
{
    [Header("Core References")]
    [SerializeField] private EnemyHealth health;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private WorldServices worldServices;

    [Header("Behaviours")]
    [SerializeField] private EnemyMovementBehaviour movementBehaviour;
    [SerializeField] private EnemyAttackBehaviour[] attackBehaviours;
    [SerializeField] private EnemyRuleBehaviour[] ruleBehaviours;

    [Header("Targeting")]
    [SerializeField] private Transform targetOverride;
    [SerializeField] private bool autoFindPlayerTarget = true;

    private EnemyRuntimeContext context;
    private bool movementSuppressed;
    private Coroutine knockbackRoutine;

    void Awake()
    {
        EnsureCoreReferences();
        AutoAssignBehaviours();

        context = new EnemyRuntimeContext(this, transform, rb, health, worldServices);
        InitializeBehaviours();
    }

    void Start()
    {
        if (targetOverride != null)
        {
            SetTarget(targetOverride);
            return;
        }

        if (!autoFindPlayerTarget) return;
        PlayerMovement player = FindFirstObjectByType<PlayerMovement>();
        if (player != null)
        {
            SetTarget(player.transform);
        }
    }

    void Update()
    {
        if (context == null) return;
        float dt = Time.deltaTime;

        if (attackBehaviours != null)
        {
            for (int i = 0; i < attackBehaviours.Length; i++)
            {
                EnemyAttackBehaviour attack = attackBehaviours[i];
                if (attack == null || !attack.IsInitialized) continue;
                attack.TickAttack(dt);
            }
        }

        if (ruleBehaviours != null)
        {
            for (int i = 0; i < ruleBehaviours.Length; i++)
            {
                EnemyRuleBehaviour rule = ruleBehaviours[i];
                if (rule == null || !rule.IsInitialized) continue;
                rule.TickRule(dt);
            }
        }
    }

    void FixedUpdate()
    {
        if (movementSuppressed) return;
        if (movementBehaviour == null || !movementBehaviour.IsInitialized) return;

        movementBehaviour.TickMovement(Time.fixedDeltaTime);
    }

    void OnDisable()
    {
        if (knockbackRoutine != null)
        {
            StopCoroutine(knockbackRoutine);
            knockbackRoutine = null;
        }

        ShutdownBehaviours();
    }

    public void SetTarget(Transform target)
    {
        if (context == null) return;
        context.Target = target;
    }

    public void ApplyKnockback(Vector2 direction, float distance, float duration)
    {
        if (knockbackRoutine != null)
        {
            StopCoroutine(knockbackRoutine);
        }

        knockbackRoutine = StartCoroutine(KnockbackRoutine(direction, distance, duration));
    }

    private IEnumerator KnockbackRoutine(Vector2 direction, float distance, float duration)
    {
        movementSuppressed = true;

        Vector2 knockbackDirection = direction.sqrMagnitude > 0.0001f ? direction.normalized : Vector2.right;
        float speed = distance / Mathf.Max(0.01f, duration);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (rb != null)
            {
                rb.linearVelocity = knockbackDirection * speed;
            }
            else
            {
                transform.position += (Vector3)(knockbackDirection * speed * Time.fixedDeltaTime);
            }

            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        movementSuppressed = false;
        knockbackRoutine = null;
    }

    private void EnsureCoreReferences()
    {
        if (health == null)
        {
            health = GetComponent<EnemyHealth>();
        }

        if (health == null)
        {
            health = GetComponentInChildren<EnemyHealth>();
        }

        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        if (worldServices == null)
        {
            worldServices = FindFirstObjectByType<WorldServices>();
        }
    }

    private void AutoAssignBehaviours()
    {
        if (movementBehaviour == null)
        {
            movementBehaviour = GetComponentInChildren<EnemyMovementBehaviour>();
        }

        if (attackBehaviours == null || attackBehaviours.Length == 0)
        {
            attackBehaviours = GetComponentsInChildren<EnemyAttackBehaviour>();
        }

        if (ruleBehaviours == null || ruleBehaviours.Length == 0)
        {
            ruleBehaviours = GetComponentsInChildren<EnemyRuleBehaviour>();
        }
    }

    private void InitializeBehaviours()
    {
        if (context == null) return;

        if (movementBehaviour != null)
        {
            movementBehaviour.Initialize(context);
        }

        if (attackBehaviours != null)
        {
            for (int i = 0; i < attackBehaviours.Length; i++)
            {
                EnemyAttackBehaviour attack = attackBehaviours[i];
                if (attack == null) continue;
                attack.Initialize(context);
            }
        }

        if (ruleBehaviours != null)
        {
            for (int i = 0; i < ruleBehaviours.Length; i++)
            {
                EnemyRuleBehaviour rule = ruleBehaviours[i];
                if (rule == null) continue;
                rule.Initialize(context);
            }
        }
    }

    private void ShutdownBehaviours()
    {
        if (movementBehaviour != null && movementBehaviour.IsInitialized)
        {
            movementBehaviour.Shutdown();
        }

        if (attackBehaviours != null)
        {
            for (int i = 0; i < attackBehaviours.Length; i++)
            {
                EnemyAttackBehaviour attack = attackBehaviours[i];
                if (attack == null || !attack.IsInitialized) continue;
                attack.Shutdown();
            }
        }

        if (ruleBehaviours != null)
        {
            for (int i = 0; i < ruleBehaviours.Length; i++)
            {
                EnemyRuleBehaviour rule = ruleBehaviours[i];
                if (rule == null || !rule.IsInitialized) continue;
                rule.Shutdown();
            }
        }
    }
}

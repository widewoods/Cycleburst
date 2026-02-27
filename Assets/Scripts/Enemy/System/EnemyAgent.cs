using UnityEngine;

[DisallowMultipleComponent]
public class EnemyAgent : MonoBehaviour
{

    private WorldServices world;
    [SerializeField] private EnemyHealth health;
    [SerializeField] private Rigidbody2D rb;

    [Header("Behaviors")]
    [SerializeField] private EnemyMovementBehavior movementBehavior;
    [SerializeField] private EnemyAttackBehavior[] attackBehaviors;


    [SerializeField] private Transform target;

    private EnemyRuntimeContext context;

    void Awake()
    {
        EnsureReferences();
        context = new EnemyRuntimeContext(this, world, transform, health, rb);

        // Initialize Composition
        EnsureBehaviors();
        InitializeBehaviors();
    }

    void Start()
    {
        PlayerMovement player = FindFirstObjectByType<PlayerMovement>();
        if (player != null)
        {
            target = player.transform;
            context.Target = target;
            return;
        }

        if (target != null)
        {
            context.Target = target;
        }
    }

    void Update()
    {
        // Tick behaviors except physics related
        float dt = Time.deltaTime;
        if (attackBehaviors != null)
        {
            foreach (var behavior in attackBehaviors)
            {
                if (behavior == null) continue;
                behavior.TickAttack(dt);
            }
        }
    }

    void FixedUpdate()
    {
        // Tick physics related behaviors
        if (movementBehavior == null) return;
        float dt = Time.fixedDeltaTime;
        movementBehavior.TickMovement(dt);
    }

    void OnDisable()
    {
        Terminate();
    }

    private void EnsureReferences()
    {
        if (world == null)
        {
            world = FindFirstObjectByType<WorldServices>();
        }
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
    }

    private void EnsureBehaviors()
    {
        if (movementBehavior == null)
        {
            movementBehavior = GetComponentInChildren<EnemyMovementBehavior>();
        }
        if (attackBehaviors == null || attackBehaviors.Length == 0)
        {
            attackBehaviors = GetComponentsInChildren<EnemyAttackBehavior>();
        }
    }

    private void InitializeBehaviors()
    {
        if (movementBehavior != null)
        {
            movementBehavior.Initialize(context);
        }
        if (attackBehaviors != null)
        {
            foreach (EnemyAttackBehavior attack in attackBehaviors)
            {
                if (attack == null) continue;
                attack.Initialize(context);
            }
        }
    }

    private void Terminate()
    {
        if (movementBehavior != null)
        {
            movementBehavior.Terminate();
        }
        if (attackBehaviors != null)
        {
            foreach (var behavior in attackBehaviors)
            {
                if (behavior == null) continue;
                behavior.Terminate();
            }
        }
    }

    public void SetTarget(Transform targetTransform)
    {
        target = targetTransform;
        if (context != null)
        {
            context.Target = targetTransform;
        }
    }

}

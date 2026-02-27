using UnityEngine;

public sealed class EnemyRuntimeContext
{
    public EnemyRuntimeContext(EnemyAgent agent, WorldServices world, Transform self, EnemyHealth health, Rigidbody2D body)
    {
        Agent = agent;
        World = world;
        Self = self;
        Body = body;
        Health = health;
        State = new EnemySharedState();
    }

    public EnemyAgent Agent { get; }
    public Transform Self { get; }
    public WorldServices World { get; }
    public EnemyHealth Health { get; }
    public Rigidbody2D Body { get; }
    public Transform Target { get; set; }
    public EnemySharedState State { get; set; }
}

public sealed class EnemySharedState
{
    public bool LockMovement { get; set; }

    public EnemySharedState()
    {
        LockMovement = false;
    }
}

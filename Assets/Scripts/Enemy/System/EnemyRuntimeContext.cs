using UnityEngine;

public sealed class EnemyRuntimeContext
{
    public EnemyRuntimeContext(EnemyAgent agent, Transform self, Rigidbody2D body, EnemyHealth health, WorldServices worldServices)
    {
        Agent = agent;
        Self = self;
        Body = body;
        Health = health;
        World = worldServices;
    }

    public EnemyAgent Agent { get; }
    public Transform Self { get; }
    public Rigidbody2D Body { get; }
    public EnemyHealth Health { get; }
    public WorldServices World { get; }

    public Transform Target { get; internal set; }
    public bool HasValidTarget => Target != null;
}

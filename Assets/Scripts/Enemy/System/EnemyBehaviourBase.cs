using UnityEngine;

public abstract class EnemyBehaviourBase : MonoBehaviour
{
    protected EnemyRuntimeContext Context { get; private set; }
    public bool IsInitialized => Context != null;

    public virtual void Initialize(EnemyRuntimeContext context)
    {
        Context = context;
        OnInitialized();
    }

    public virtual void Shutdown()
    {
        OnShutdown();
        Context = null;
    }

    protected virtual void OnInitialized()
    {
    }

    protected virtual void OnShutdown()
    {
    }
}

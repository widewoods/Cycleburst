using UnityEngine;

public abstract class EnemyBehaviorBase : MonoBehaviour
{
    protected EnemyRuntimeContext Context { get; private set; }


    public virtual void Initialize(EnemyRuntimeContext context)
    {
        Context = context;
        OnInitialize();
    }

    public virtual void Terminate()
    {
        OnTerminate();
    }

    protected virtual void OnInitialize() { }

    protected virtual void OnTerminate() { }
}

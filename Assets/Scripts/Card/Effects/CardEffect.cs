using System.Collections;
using UnityEngine;

public abstract class CardEffect : ScriptableObject
{
    // Optional pre-check (range, LOS, etc.)
    public virtual bool CanResolve(CardContext ctx) => true;

    // Do the effect
    public abstract void Resolve(CardContext ctx);

    public virtual IEnumerator ResolveSequence(CardContext ctx)
    {
        Resolve(ctx);
        yield break;
    }

}
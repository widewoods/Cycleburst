using UnityEngine;

public abstract class CardEffect : ScriptableObject
{
    // Optional pre-check (range, LOS, etc.)
    public virtual bool CanResolve(CardContext ctx) => true;

    // Do the effect
    public abstract void Resolve(CardContext ctx);
}
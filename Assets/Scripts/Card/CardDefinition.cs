using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Card Definition")]
public class CardDefinition : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string id;
    [SerializeField] private string displayName;
    [SerializeField] private Sprite icon;

    [Header("Gameplay")]
    [SerializeField] private bool resolveSimultaneous = true;
    [SerializeField] private int heatGenerated;

    [Header("Composition")]
    [SerializeField] private List<CardEffect> effects = new();

    public string Id => id;
    public string DisplayName => displayName;
    public Sprite Icon => icon;
    public bool ResolveSimultaneous => resolveSimultaneous;
    public int HeatGenerated => heatGenerated;
    public IReadOnlyList<CardEffect> Effects => effects;

    public bool CanPlay(CardContext ctx)
    {
        foreach (var e in effects)
            if (!e.CanResolve(ctx)) return false;
        return true;
    }

    public void Resolve(CardContext ctx)
    {
        foreach (var e in effects)
        {
            if (ctx.caster != null)
                e.Resolve(ctx);
        }
    }

    public IEnumerator ResolveSequence(CardContext ctx)
    {
        foreach (var effect in effects)
            yield return effect.ResolveSequence(ctx);
    }

}

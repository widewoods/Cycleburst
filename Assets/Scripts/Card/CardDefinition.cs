using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Card Definition")]
public class CardDefinition : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string id;
    [SerializeField] private string displayName;
    [SerializeField] private Sprite icon;

    // [Header("Gameplay")]
    // [SerializeField] private float cooldown;

    [Header("Composition")]
    [SerializeField] private List<CardEffect> effects = new();

    public string Id => id;
    public string DisplayName => displayName;
    public Sprite Icon => icon;
    // public float Cooldown => cooldown;
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
            e.Resolve(ctx);
    }
}

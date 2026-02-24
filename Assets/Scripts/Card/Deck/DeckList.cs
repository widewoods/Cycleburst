using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Deck")]
public class DeckList : ScriptableObject
{
    public List<CardDefinition> cards;
}

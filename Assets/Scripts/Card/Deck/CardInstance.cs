using System;
using UnityEngine;

[Serializable]
public class CardInstance
{
    private CardDefinition cardDefinition;
    public CardDefinition Definition => cardDefinition;
    public CardInstance(CardDefinition def) { cardDefinition = def; }
}

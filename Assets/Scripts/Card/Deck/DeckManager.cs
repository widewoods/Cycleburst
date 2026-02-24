using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    [SerializeField] private WorldServices worldServices;
    [SerializeField] private DeckList deckList;
    private List<CardInstance> drawPile;
    private List<CardInstance> discardPile;

    private CardInstance[] hand;

    // Player
    [SerializeField] private Transform playerTransform;
    [SerializeField] private PlayerAim playerAim;

    void Start()
    {
        drawPile = new List<CardInstance>();
        discardPile = new List<CardInstance>();
        InitFromDeckList(deckList);
    }


    public bool TryPlaySlot(int slot)
    {
        var card = hand[slot];
        if (card == null)
        {
            Debug.Log($"Hand in slot {slot} is empty");
            return false;
        }

        var ctx = BuildContext(card);          // player, aim dir, cursor pos, etc.
        if (!card.Definition.CanPlay(ctx)) return false;

        card.Definition.Resolve(ctx);          // runs effect list
        Debug.Log($"Played {card.Definition.Id} in slot {slot}");

        Discard(slot);
        DrawToSlot(slot);

        return true;
    }

    private void DrawToSlot(int slot)
    {
        if (drawPile.Count == 0)
        {
            if (!TryRefillFromDiscard()) return;
        }

        int lastIndex = drawPile.Count - 1;
        var card = drawPile[lastIndex];
        drawPile.RemoveAt(lastIndex);
        hand[slot] = card;
        Debug.Log($"Drew {hand[slot].Definition.Id}");
    }

    private void Discard(int slot)
    {
        discardPile.Add(hand[slot]);
        Debug.Log($"Discarded {hand[slot].Definition.Id}");
        hand[slot] = null;
    }

    private void InitFromDeckList(DeckList deckList)
    {
        drawPile.Clear(); discardPile.Clear();
        hand = new CardInstance[4];

        foreach (var def in deckList.cards)
            drawPile.Add(new CardInstance(def));

        drawPile.Shuffle();

        for (int i = 0; i < hand.Length; i++)
            DrawToSlot(i);
    }

    private bool TryRefillFromDiscard()
    {
        if (discardPile.Count == 0) return false;

        drawPile = new List<CardInstance>(discardPile);
        discardPile.Clear();
        drawPile.Shuffle();
        return true;
    }

    public void AddCardToDeck(CardDefinition def)
    {
        discardPile.Add(new CardInstance(def));
    }

    CardContext BuildContext(CardInstance card)
    {
        return new CardContext
        {
            caster = playerTransform,
            aimDir = playerAim.aimDirection,
            world = worldServices
        };
    }
}


public static class ListExtensions
{
    public static void Shuffle<T>(this IList<T> list)
    {
        int count = list.Count;
        for (int i = 0; i < count; i++)
        {
            // Choose a random index from the current position to the end
            int randomIndex = Random.Range(i, count);

            // Swap the elements
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}

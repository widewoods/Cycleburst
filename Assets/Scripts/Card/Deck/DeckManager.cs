using System.Collections.Generic;
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
            return false;
        }

        var ctx = BuildContext(card);          // player, aim dir, cursor pos, etc.
        if (!card.Definition.CanPlay(ctx)) return false;

        card.Definition.Resolve(ctx);          // runs effect list

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

    }

    private void Discard(int slot)
    {
        discardPile.Add(hand[slot]);

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

    public int HandSlotCount => hand == null ? 0 : hand.Length;
    public int DrawPileCount => drawPile == null ? 0 : drawPile.Count;
    public int DiscardPileCount => discardPile == null ? 0 : discardPile.Count;

    public CardInstance GetCardInHandSlot(int slot)
    {
        if (hand == null) return null;
        if (slot < 0 || slot >= hand.Length) return null;
        return hand[slot];
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

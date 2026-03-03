using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private WorldServices worldServices;
    [SerializeField] private PlayerHeatSystem playerHeat;
    [SerializeField] private DeckList deckList;

    private List<CardInstance> drawPile;
    private List<CardInstance> discardPile;

    private CardInstance[] hand;
    [SerializeField] private int handSize;
    private bool isResolvingSequence;
    private Coroutine playRoutine;

    // Player
    [SerializeField] private Transform playerTransform;
    [SerializeField] private PlayerAim playerAim;




    [SerializeField] private float globalCooldown;
    private float globalCooldownRemaining;

    void Start()
    {
        drawPile ??= new List<CardInstance>();
        discardPile ??= new List<CardInstance>();
        InitFromDeckList(deckList);
    }

    void Update()
    {
        if (globalCooldownRemaining > 0)
        {
            globalCooldownRemaining = Mathf.Max(0f, globalCooldownRemaining - Time.deltaTime);
        }
    }

    void OnEnable()
    {
        if (worldServices != null && worldServices.Wave != null)
        {
            worldServices.Wave.WaveCleared += InitFromCurrentDeck;
        }
    }

    void OnDisable()
    {
        if (worldServices != null && worldServices.Wave != null)
        {
            worldServices.Wave.WaveCleared -= InitFromCurrentDeck;
        }

        if (playRoutine != null)
        {
            StopCoroutine(playRoutine);
            playRoutine = null;
        }
        isResolvingSequence = false;
    }


    public bool TryPlaySlot(int slot)
    {
        if (hand == null) return false;
        if (slot < 0 || slot >= hand.Length) return false;
        if (isResolvingSequence) return false;
        if (globalCooldownRemaining > 0f) return false;

        var card = hand[slot];

        if (!CanPlayCard(card)) return false;

        var ctx = BuildContext(card);
        if (!card.Definition.CanPlay(ctx))
        {
            return false;
        }

        globalCooldownRemaining = globalCooldown;

        if (!card.Definition.ResolveSimultaneous)
        {
            isResolvingSequence = true;
            playRoutine = StartCoroutine(PlayCardRoutine(card, slot, ctx));
            return true;
        }
        PlayCard(card, slot, ctx);
        return true;
    }

    private bool CanPlayCard(CardInstance card)
    {
        if (card == null)
        {
            return false;
        }
        if (card.Definition == null)
        {
            Debug.LogWarning($"Played card has a card instance with null definition.");
            return false;
        }

        if (playerHeat != null && playerHeat.Overheated)
        {
            Debug.Log("Overheated!");
            return false;
        }

        return true;
    }

    private IEnumerator PlayCardRoutine(CardInstance card, int slot, CardContext ctx)
    {
        yield return card.Definition.ResolveSequence(ctx);

        ApplyPostPlay(card, slot);
        isResolvingSequence = false;
        playRoutine = null;
    }

    private void PlayCard(CardInstance card, int slot, CardContext ctx)
    {
        card.Definition.Resolve(ctx);          // runs effect list
        ApplyPostPlay(card, slot);
    }

    private void ApplyPostPlay(CardInstance card, int slot)
    {
        if (playerHeat != null)
        {
            playerHeat.ChangeHeat(+card.Definition.HeatGenerated);
        }

        if (hand == null || slot < 0 || slot >= hand.Length) return;
        if (hand[slot] != card) return;

        Discard(slot);
        DrawToSlot(slot);
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
        if (deckList == null) return;
        if (drawPile == null) drawPile = new List<CardInstance>();
        if (discardPile == null) discardPile = new List<CardInstance>();

        drawPile.Clear(); discardPile.Clear();
        hand = new CardInstance[handSize];

        foreach (var def in deckList.cards)
            drawPile.Add(new CardInstance(def));

        drawPile.Shuffle();

        for (int i = 0; i < hand.Length; i++)
            DrawToSlot(i);
    }

    private void InitFromCurrentDeck(int wave)
    {
        if (hand != null)
        {
            for (int i = 0; i < hand.Length; i++)
            {
                if (hand[i] != null)
                    drawPile.Add(hand[i]);
            }
        }

        drawPile.AddRange(discardPile);
        discardPile.Clear();
        hand = new CardInstance[handSize];

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
        Transform caster = playerTransform;
        Vector2 aimDirection = playerAim.aimDirection;
        if (aimDirection.sqrMagnitude < 0.0001f)
        {
            aimDirection = Vector2.up;
        }

        return new CardContext
        {
            caster = caster,
            aimDir = aimDirection,
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

using UnityEngine;

public class CardInputRouter : MonoBehaviour
{
    [SerializeField] private DeckManager deckManager;

    void OnPlayQ()
    {
        deckManager.TryPlaySlot(0);
    }

    void OnPlayE()
    {
        deckManager.TryPlaySlot(1);
    }

    void OnPlayR()
    {
        deckManager.TryPlaySlot(2);
    }

    void OnPlayF()
    {
        deckManager.TryPlaySlot(3);
    }
}

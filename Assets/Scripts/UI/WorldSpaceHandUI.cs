using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorldSpaceHandUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DeckManager deckManager;
    [SerializeField] private Transform followTarget;
    [SerializeField] private TextMeshProUGUI handText;

    [Header("Layout")]
    [SerializeField] private Vector3 worldOffset = new Vector3(0f, 1.5f, 0f);
    [SerializeField] private bool faceCamera = true;
    [SerializeField] private bool hideWhenEmpty = false;

    [Header("Display")]
    [SerializeField] private bool showSlotKeys = true;
    [SerializeField] private bool showEmptySlots = false;

    private static readonly string[] DefaultSlotKeys = { "Q", "E", "R", "F" };

    private readonly StringBuilder builder = new StringBuilder(128);
    private Camera cachedCamera;

    void Awake()
    {
        if (deckManager == null)
        {
            deckManager = FindFirstObjectByType<DeckManager>();
        }

        if (followTarget == null && deckManager != null)
        {
            followTarget = deckManager.transform;
        }

        if (handText == null)
        {
            handText = GetComponentInChildren<TextMeshProUGUI>();
        }

        cachedCamera = Camera.main;
    }

    void LateUpdate()
    {
        UpdateWorldPosition();
        UpdateFacing();
        RefreshText();
    }

    private void UpdateWorldPosition()
    {
        if (followTarget == null) return;
        transform.position = followTarget.position + worldOffset;
    }

    private void UpdateFacing()
    {
        if (!faceCamera) return;

        if (cachedCamera == null)
        {
            cachedCamera = Camera.main;
        }
        if (cachedCamera == null) return;

        transform.rotation = cachedCamera.transform.rotation;
    }

    private void RefreshText()
    {
        if (handText == null) return;
        if (deckManager == null)
        {
            handText.text = "No Deck";
            return;
        }

        builder.Clear();

        int slotCount = deckManager.HandSlotCount;
        for (int i = 0; i < slotCount; i++)
        {
            CardInstance card = deckManager.GetCardInHandSlot(i);
            if (card == null && !showEmptySlots)
            {
                continue;
            }

            if (builder.Length > 0)
            {
                builder.AppendLine();
            }

            if (showSlotKeys)
            {
                builder.Append('[')
                    .Append(GetSlotKey(i))
                    .Append("] ");
            }

            if (card == null || card.Definition == null)
            {
                builder.Append("(empty)");
            }
            else
            {
                builder.Append(ResolveCardName(card.Definition));
            }
        }

        bool noVisibleCards = builder.Length == 0;
        handText.text = noVisibleCards ? string.Empty : builder.ToString();
        handText.enabled = !hideWhenEmpty || !noVisibleCards;
    }

    private static string GetSlotKey(int slot)
    {
        if (slot >= 0 && slot < DefaultSlotKeys.Length)
        {
            return DefaultSlotKeys[slot];
        }

        return (slot + 1).ToString();
    }

    private static string ResolveCardName(CardDefinition definition)
    {
        if (definition == null) return "(null)";
        if (!string.IsNullOrWhiteSpace(definition.DisplayName)) return definition.DisplayName;
        if (!string.IsNullOrWhiteSpace(definition.Id)) return definition.Id;
        return definition.name;
    }
}

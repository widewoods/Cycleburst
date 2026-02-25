using System.Text;
using UnityEngine;

public class HandDebugUI : MonoBehaviour
{
    [SerializeField] private DeckManager deckManager;
    [SerializeField] private Vector2 screenPosition = new Vector2(12f, 12f);
    [SerializeField] private int fontSize = 16;
    [SerializeField] private Color textColor = Color.white;
    [SerializeField] private Color backgroundColor = new Color(0f, 0f, 0f, 0.75f);

    private readonly string[] slotKeys = { "Q", "E", "R", "F" };
    private readonly StringBuilder builder = new StringBuilder(256);
    private GUIStyle textStyle;
    private GUIStyle panelStyle;

    private void Awake()
    {
        if (deckManager == null)
        {
            deckManager = FindFirstObjectByType<DeckManager>();
        }
    }

    private void OnGUI()
    {
        EnsureStyles();

        if (deckManager == null)
        {
            GUI.Label(new Rect(screenPosition.x, screenPosition.y, 500f, 40f), "Hand UI: DeckManager reference is missing.");
            return;
        }

        if (textStyle.fontSize != fontSize)
        {
            textStyle.fontSize = fontSize;
        }
        textStyle.normal.textColor = textColor;

        builder.Clear();
        builder.AppendLine("=== Hand Debug ===");
        builder.Append("Draw: ").Append(deckManager.DrawPileCount)
            .Append(" | Discard: ").Append(deckManager.DiscardPileCount)
            .AppendLine();

        int slotCount = deckManager.HandSlotCount;
        for (int i = 0; i < slotCount; i++)
        {
            CardInstance card = deckManager.GetCardInHandSlot(i);
            string key = i < slotKeys.Length ? slotKeys[i] : (i + 1).ToString();
            string name = card == null ? "(empty)" : ResolveCardName(card.Definition);
            builder.Append('[').Append(key).Append("] ").Append(name).AppendLine();
        }

        Rect panelRect = new Rect(screenPosition.x, screenPosition.y, 560f, 220f);
        Color previousColor = GUI.color;
        GUI.color = backgroundColor;
        GUI.Box(panelRect, GUIContent.none, panelStyle);
        GUI.color = previousColor;

        GUI.Label(panelRect, builder.ToString(), textStyle);
    }

    private void EnsureStyles()
    {
        if (textStyle == null)
        {
            textStyle = new GUIStyle(GUI.skin.label);
        }

        textStyle.fontSize = fontSize;
        textStyle.normal.textColor = textColor;

        if (panelStyle == null)
        {
            panelStyle = new GUIStyle(GUI.skin.box);
            panelStyle.normal.background = Texture2D.whiteTexture;
        }
    }

    private static string ResolveCardName(CardDefinition definition)
    {
        if (definition == null) return "(null)";
        if (!string.IsNullOrWhiteSpace(definition.DisplayName)) return definition.DisplayName;
        if (!string.IsNullOrWhiteSpace(definition.Id)) return definition.Id;
        return definition.name;
    }
}

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IntermissionReward : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DeckList rewards;
    [SerializeField] private DeckManager deck;
    [SerializeField] private IntermissionManager intermission;

    [Header("Buttons")]
    [SerializeField] private RewardButton[] rewardButtons;

    private readonly int REWARD_NUM = 3;

    void OnEnable()
    {
        intermission.intermissionStarted += HandleIntermissionStart;
        intermission.intermissionEnded += HandleIntermssionEnd;
    }

    void OnDisable()
    {
        intermission.intermissionStarted -= HandleIntermissionStart;
        intermission.intermissionEnded -= HandleIntermssionEnd;
    }

    void HandleIntermissionStart()
    {
        CardDefinition[] rewardCandidates = ChooseRewardCandidates();
        int displayCount = Mathf.Min(rewardButtons.Length, rewardCandidates.Length);

        for (int i = 0; i < displayCount; i++)
        {
            RewardButton rewardButton = rewardButtons[i];
            CardDefinition chosenCard = rewardCandidates[i];

            rewardButton.button.onClick.RemoveAllListeners();

            if (chosenCard == null)
            {
                rewardButton.button.interactable = false;
                rewardButton.text.text = "Unavailable";
                continue;
            }

            rewardButton.button.interactable = true;
            rewardButton.text.text = chosenCard.DisplayName;
            rewardButton.button.onClick.AddListener(() => AddCard(chosenCard));
            rewardButton.button.onClick.AddListener(intermission.FinishIntermission);
        }

        for (int i = displayCount; i < rewardButtons.Length; i++)
        {
            RewardButton rewardButton = rewardButtons[i];
            rewardButton.button.onClick.RemoveAllListeners();
            rewardButton.button.interactable = false;
            rewardButton.text.text = "Unavailable";
        }
    }


    void HandleIntermssionEnd()
    {
        foreach (RewardButton button in rewardButtons)
        {
            button.button.onClick.RemoveAllListeners();
        }
    }

    CardDefinition[] ChooseRewardCandidates()
    {
        if (rewards == null || rewards.cards == null || rewards.cards.Count == 0)
        {
            return Array.Empty<CardDefinition>();
        }

        int rewardCount = Mathf.Min(REWARD_NUM, rewards.cards.Count);
        int remaining = rewardCount;
        CardDefinition[] rewardCandidates = new CardDefinition[rewardCount];

        for (int i = rewards.cards.Count - 1; i >= 0; i--)
        {
            bool chosen = UnityEngine.Random.Range(0, i + 1) < remaining;
            if (chosen)
            {
                remaining -= 1;
                rewardCandidates[remaining] = rewards.cards[i];
            }

            if (remaining == 0) break;
        }
        return rewardCandidates;
    }

    void AddCard(CardDefinition card)
    {
        if (card == null) return;
        deck.AddCardToDeck(card);
    }

    [Serializable]
    private struct RewardButton
    {
        public Button button;
        public TextMeshProUGUI text;
    }
}

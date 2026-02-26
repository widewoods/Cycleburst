using UnityEngine;
using UnityEngine.UI;

public class HeatBarUI : MonoBehaviour
{
    [SerializeField] private PlayerHeatSystem playerHeat;
    [SerializeField] private Image heatBarImage;

    void OnEnable()
    {
        playerHeat.OnHeatChange += HandleHeatChange;
    }

    private void HandleHeatChange(int currentHeat)
    {
        heatBarImage.fillAmount = (float)currentHeat / playerHeat.MaxHeat;
    }
}

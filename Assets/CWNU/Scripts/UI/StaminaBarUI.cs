using UnityEngine;
using TMPro;

public class StaminaBarUI : MonoBehaviour
{
    [Header("Stamina 텍스트 (TextMeshPro)")]
    public TextMeshProUGUI staminaText;

    void Update()
    {
        if (PlayerStats.Instance == null || staminaText == null) return;
        staminaText.text = $"{(int)PlayerStats.Instance.currentStamina} / {(int)PlayerStats.Instance.maxStamina}";
    }
}

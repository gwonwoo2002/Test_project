using UnityEngine;
using TMPro;

public class HealthBarUI : MonoBehaviour
{
    [Header("HP 텍스트 (TextMeshPro)")]
    public TextMeshProUGUI hpText;

    void Update()
    {
        if (PlayerStats.Instance == null || hpText == null) return;
        hpText.text = $"{(int)PlayerStats.Instance.currentHP} / {(int)PlayerStats.Instance.maxHP}";
    }
}

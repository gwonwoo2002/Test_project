using UnityEngine;
using TMPro;

public class MoneyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneyText;

    void Update()
    {
        if (PlayerStats.Instance == null || moneyText == null) return;
        moneyText.text = $"${(int)PlayerStats.Instance.currentMoney} / {(int)PlayerStats.Instance.goalMoney}";
    }
}

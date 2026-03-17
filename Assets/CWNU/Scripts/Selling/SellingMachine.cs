using UnityEngine;
using UnityEngine.InputSystem;

public class SellingMachine : MonoBehaviour
{
    [Header("Settings")]
    public float interactRange = 2f;
    public float sellHoldTime = 1.5f;

    // PlayerDrop에서 슬롯 1 드롭 억제에 사용
    public static bool PlayerInRange = false;

    private Transform player;
    private float holdTimer = 0f;

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    void Update()
    {
        if (player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);
        PlayerInRange = dist <= interactRange;

        if (!PlayerInRange)
        {
            holdTimer = 0f;
            return;
        }

        if (Keyboard.current != null && Keyboard.current[Key.Digit1].isPressed)
        {
            holdTimer += Time.deltaTime;
            if (holdTimer >= sellHoldTime)
            {
                TrySell(0);
                holdTimer = 0f;
            }
        }
        else
        {
            holdTimer = 0f;
        }
    }

    void TrySell(int slotIndex)
    {
        ItemData item = InventoryManager.Instance.GetItem(slotIndex);
        if (item == null)
        {
            Debug.Log("[Sell] Slot 1 is empty.");
            return;
        }

        InventoryManager.Instance.SellItem(slotIndex);
        PlayerStats.Instance.AddMoney(item.price);
        Debug.Log($"[Sell] {item.itemName} (${item.price}) sold. Money: {PlayerStats.Instance.currentMoney} / {PlayerStats.Instance.goalMoney}");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}

using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    public const int SlotCount = 4;
    private ItemData[] slots = new ItemData[SlotCount];
    private GameObject[] slotObjects = new GameObject[SlotCount];

    public event System.Action OnInventoryChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        PrintInventory();
    }

    public bool AddItem(ItemData item, GameObject source)
    {
        for (int i = 0; i < SlotCount; i++)
        {
            if (slots[i] == null)
            {
                slots[i] = item;
                slotObjects[i] = source;
                OnInventoryChanged?.Invoke();
                PrintInventory();
                return true;
            }
        }
        Debug.Log("인벤토리가 가득 찼습니다.");
        return false;
    }

    public void DropItem(int slotIndex, Vector3 dropPosition)
    {
        if (slots[slotIndex] == null) return;

        GameObject obj = slotObjects[slotIndex];
        if (obj != null)
        {
            obj.transform.position = dropPosition;
            obj.SetActive(true);
        }

        slots[slotIndex] = null;
        slotObjects[slotIndex] = null;
        OnInventoryChanged?.Invoke();
        PrintInventory();
    }

    // 판매 시 호출 — 아이템을 씬에서 완전히 제거
    public void SellItem(int slotIndex)
    {
        if (slots[slotIndex] == null) return;

        if (slotObjects[slotIndex] != null)
            Destroy(slotObjects[slotIndex]);

        slots[slotIndex] = null;
        slotObjects[slotIndex] = null;
        OnInventoryChanged?.Invoke();
        PrintInventory();
    }

    void PrintInventory()
    {
        string log = "[Inventory]";
        for (int i = 0; i < SlotCount; i++)
        {
            string name = slots[i] != null ? slots[i].itemName : "비어있음";
            log += $" [{i + 1}:{name}]";
        }
        Debug.Log(log);
    }

    public ItemData GetItem(int slotIndex) => slots[slotIndex];
}

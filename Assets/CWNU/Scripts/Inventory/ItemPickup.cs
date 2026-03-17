using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public ItemData itemData;

    // PlayerInteraction에서 호출
    public void Pickup()
    {
        if (InventoryManager.Instance.AddItem(itemData, gameObject))
        {
            gameObject.SetActive(false);
        }
    }
}

using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float pickupRange = 2f;
    public LayerMask itemLayer;

    [Header("UI")]
    public TextMeshProUGUI pressEText; // Inspector에서 연결

    private void Update()
    {
        ItemPickup nearest = FindNearestItem();

        if (pressEText != null)
        {
            pressEText.gameObject.SetActive(nearest != null);
            if (nearest != null && nearest.itemData != null)
                pressEText.text = $"{nearest.itemData.itemName} (${nearest.itemData.price})";
        }

        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame && nearest != null)
            nearest.Pickup();
    }

    private ItemPickup FindNearestItem()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, pickupRange, itemLayer);

        float closest = float.MaxValue;
        ItemPickup target = null;

        foreach (var hit in hits)
        {
            float dist = Vector3.Distance(transform.position, hit.transform.position);
            if (dist < closest)
            {
                var pickup = hit.GetComponent<ItemPickup>();
                if (pickup != null)
                {
                    closest = dist;
                    target = pickup;
                }
            }
        }

        return target;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}

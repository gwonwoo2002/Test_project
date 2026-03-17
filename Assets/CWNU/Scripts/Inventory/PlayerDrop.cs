using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDrop : MonoBehaviour
{
    public Transform cameraTransform;
    public float dropDistance = 1.5f;

    private float[] holdTimes = new float[4];
    private Key[] keys = { Key.Digit1, Key.Digit2, Key.Digit3, Key.Digit4 };

    private void Update()
    {
        if (Keyboard.current == null) return;

        for (int i = 0; i < 4; i++)
        {
            // 슬롯 1(index 0)은 SellingMachine 범위 내에서 드롭 억제
            if (i == 0 && SellingMachine.PlayerInRange)
            {
                holdTimes[i] = 0f;
                continue;
            }

            if (Keyboard.current[keys[i]].isPressed)
            {
                holdTimes[i] += Time.deltaTime;
                if (holdTimes[i] >= 0.7f)
                {
                    Vector3 dropPos = GetFloorDropPosition();
                    InventoryManager.Instance.DropItem(i, dropPos);
                    holdTimes[i] = 0f;
                }
            }
            else
            {
                holdTimes[i] = 0f;
            }
        }
    }

    private Vector3 GetFloorDropPosition()
    {
        Vector3 forward = cameraTransform.position + cameraTransform.forward * dropDistance;

        if (Physics.Raycast(forward, Vector3.down, out RaycastHit hit, 10f))
            return hit.point;

        return transform.position + transform.forward * dropDistance;
    }
}

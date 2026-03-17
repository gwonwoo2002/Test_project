using UnityEngine;
using UnityEngine.Events;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }

    [Header("Health")]
    public float maxHP = 100f;
    public float currentHP;

    [Header("Stamina")]
    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaDrainRate = 25f;       // 달리기 중 초당 감소량
    public float staminaRecoveryRate = 15f;    // 회복 시 초당 증가량
    public float staminaRecoveryDelay = 2f;    // Shift 뗀 후 회복 시작까지 대기 시간

    [Header("Money")]
    public float goalMoney = 100f;
    public float currentMoney = 0f;

    private float staminaRecoveryTimer = 0f;

    public UnityEvent onPlayerDeath;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        currentHP = maxHP;
        currentStamina = maxStamina;
    }

    public void TakeDamage(float amount)
    {
        currentHP = Mathf.Max(currentHP - amount, 0f);
        Debug.Log($"[PlayerStats] HP: {currentHP}/{maxHP}");
        if (currentHP <= 0f) Die();
    }

    void Die()
    {
        Debug.Log("[PlayerStats] Player Dead");
        onPlayerDeath?.Invoke();
        gameObject.SetActive(false);
    }

    // FPSMovement에서 매 프레임 호출
    public void UpdateStamina(bool isSprinting)
    {
        if (isSprinting && currentStamina > 0f)
        {
            currentStamina = Mathf.Max(currentStamina - staminaDrainRate * Time.deltaTime, 0f);
            staminaRecoveryTimer = 0f;
        }
        else if (!isSprinting)
        {
            staminaRecoveryTimer += Time.deltaTime;
            if (staminaRecoveryTimer >= staminaRecoveryDelay)
            {
                currentStamina = Mathf.Min(currentStamina + staminaRecoveryRate * Time.deltaTime, maxStamina);
            }
        }
    }

    public bool CanSprint() => currentStamina > 0f;

    public void AddMoney(float amount)
    {
        currentMoney += amount;
        Debug.Log($"[PlayerStats] Money: {currentMoney} / {goalMoney}");
    }
}

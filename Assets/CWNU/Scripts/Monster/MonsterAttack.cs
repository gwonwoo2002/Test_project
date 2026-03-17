using UnityEngine;

// 몬스터 오브젝트에 부착. Collider는 IsTrigger 체크 필요.
// Player 오브젝트에 Tag: "Player" 설정 필요.
public class MonsterAttack : MonoBehaviour
{
    public float damage = 50f; // 1대당 50 데미지 (3대 = 150 = Death)

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStats.Instance?.TakeDamage(damage);
        }
    }
}

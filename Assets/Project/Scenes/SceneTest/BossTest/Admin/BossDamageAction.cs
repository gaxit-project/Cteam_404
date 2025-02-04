using UnityEngine;

public class BossDamageAction : MonoBehaviour, IAdminAction
{
    [SerializeField] private EnemyHealth bossHealth;
    [SerializeField] private int damageAmount = 10;

    public void ExecuteAction()
    {
        if (bossHealth != null)
        {
            bossHealth.TakeDamage(damageAmount);
            Debug.Log("管理者モード: ボスに " + damageAmount + " ダメージ");
        }
        else
        {
            Debug.LogWarning("BossDamageAction: bossHealth が設定されていません");
        }
    }
}

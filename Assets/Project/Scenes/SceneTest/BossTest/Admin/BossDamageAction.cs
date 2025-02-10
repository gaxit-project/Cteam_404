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
            Debug.Log("�Ǘ��҃��[�h: �{�X�� " + damageAmount + " �_���[�W");
        }
        else
        {
            Debug.LogWarning("BossDamageAction: bossHealth ���ݒ肳��Ă��܂���");
        }
    }
}

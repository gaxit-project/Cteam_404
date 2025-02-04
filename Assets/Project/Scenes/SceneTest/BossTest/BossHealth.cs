using UnityEngine;

public class BossHealth : EnemyHealth
{
    [Header("�t�F�[�Y�ύXHP臒l")]
    [SerializeField] private int phaseChangeThreshold = 50;
    private bool hasPhaseChanged = false;

    protected override void Start()
    {
        base.Start();
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);

        // HP�����ȉ��Ńt�F�[�Y�`�F���W
        if (!hasPhaseChanged && currentHealth <= phaseChangeThreshold)
        {
            ChangePhase();
            hasPhaseChanged = true;
        }
    }

    private void ChangePhase()
    {
        Debug.Log("�{�X���t�F�[�Y2�Ɉڍs");
        BossAIManager bossAI = GetComponent<BossAIManager>();
        if (bossAI != null)
        {
            bossAI.SetAttackInterval(1.5f, 3.0f);�@�@//�U���p�x���㏸
        }
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }


    protected override void Die()
    {
        Debug.Log("�{�X��|����");
        base.Die();
    }
}

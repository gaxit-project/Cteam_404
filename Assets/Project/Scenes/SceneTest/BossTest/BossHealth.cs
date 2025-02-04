using UnityEngine;

public class BossHealth : EnemyHealth
{
    [Header("フェーズ変更HP閾値")]
    [SerializeField] private int phaseChangeThreshold = 50;
    private bool hasPhaseChanged = false;

    protected override void Start()
    {
        base.Start();
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);

        // HPが一定以下でフェーズチェンジ
        if (!hasPhaseChanged && currentHealth <= phaseChangeThreshold)
        {
            ChangePhase();
            hasPhaseChanged = true;
        }
    }

    private void ChangePhase()
    {
        Debug.Log("ボスがフェーズ2に移行");
        BossAIManager bossAI = GetComponent<BossAIManager>();
        if (bossAI != null)
        {
            bossAI.SetAttackInterval(1.5f, 3.0f);　　//攻撃頻度が上昇
        }
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }


    protected override void Die()
    {
        Debug.Log("ボスを倒した");
        base.Die();
    }
}

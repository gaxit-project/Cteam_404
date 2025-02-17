using UnityEngine;

public class BossHealth : EnemyHealth
{
    [Header("第二形態")]
    [SerializeField] private int phaseTwoThreshold;
    private bool isPhaseTwo = false;
    private BossStateAI bossStateAI;
    private BossHealthUI bossHealthUI;

    protected override void Start()
    {
        base.Start();
        bossStateAI =  GetComponent<BossStateAI>();
        bossHealthUI = FindObjectOfType<BossHealthUI>();
    }

    /// <summary>
    /// テスト用のため後日削除
    /// </summary>
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("ボスに10ダメージ");
            TakeDamage(10);
        }
    }

    public override void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return;

        base.TakeDamage(damage);

        Debug.Log($"[BossHealth] HP: {currentHealth}, フェーズ閾値: {phaseTwoThreshold}");

        if(!isPhaseTwo && currentHealth <= phaseTwoThreshold)
        {
            EnterPhaseTwo();
        }
    }

    protected override void Die()
    {
        base.Die();
        if(bossHealthUI != null)
        {
            bossHealthUI.DestroyHealthBar();
        }
    }

    private void EnterPhaseTwo()
    {
        isPhaseTwo = true;

        if(bossStateAI != null)
        {
            bossStateAI.EnterSecondPhase();
        }
    }
}   
using UnityEngine;

public class BossHealth : EnemyHealth
{
    [Header("第二形態")]
    [SerializeField] private int phaseTwoThreshold;
    private bool isPhaseTwo = false;
    private BossStateAI bossStateAI;

    protected override void Start()
    {
        base.Start();
        bossStateAI =  GetComponent<BossStateAI>();
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

    private void EnterPhaseTwo()
    {
        isPhaseTwo = true;

        if(bossStateAI != null)
        {
            bossStateAI.EnterSecondPhase();
        }
    }
}   
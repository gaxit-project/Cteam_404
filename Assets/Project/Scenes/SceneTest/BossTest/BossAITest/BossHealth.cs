using UnityEngine;
using UnityEngine.SceneManagement;

public class BossHealth : EnemyHealth
{
    [Header("第二形態")]
    [SerializeField] private int phaseTwoThreshold;
    [Header("第三形態")]
    [SerializeField] private int phaseThreeThreshold;
    private bool isPhaseSecond = false;
    private bool isPhaseThird = false;
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
        if (Input.GetKeyDown(KeyCode.O))
        {
            TakeDamage(50);
        }
    }

    public override void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return;

        base.TakeDamage(damage);

        if(!isPhaseSecond && currentHealth <= phaseTwoThreshold)
        {
            EnterPhaseSecond();
        }

        if(!isPhaseThird && currentHealth <= phaseThreeThreshold)
        {
            EnterPhaseThird();
        }
    }

    protected override void Die()
    {
        base.Die();
        if(bossHealthUI != null)
        {
            bossHealthUI.DestroyHealthBar();
        }

        SceneManager.LoadScene("GameClear");
    }

    private void EnterPhaseSecond()
    {
        isPhaseSecond = true;

        if(bossStateAI != null)
        {
            bossStateAI.EnterSecondPhase();
        }
    }

    private void EnterPhaseThird()
    {
        isPhaseThird = true;
        isPhaseSecond = false;

        if(bossStateAI != null)
        {
            bossStateAI.EnterThirdPhase();
        }
    }
}   
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading;
using System.Collections;

public class BossHealth : EnemyHealth
{
    [Header("第二形態")]
    [SerializeField] private int phaseTwoThreshold;
    [Header("第三形態")]
    [SerializeField] private int phaseThreeThreshold;
    [Header("爆発エフェクト")]
    [SerializeField] private GameObject[] explosionObject;
    public bool isPhaseSecond = false;
    public bool isPhaseThird = false;
    private BossStateAI bossStateAI;
    private BossHealthUI bossHealthUI;
    private BossFace bossFace;

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
        AudioManager.GetInstance().PlaySound(17);

        base.TakeDamage(damage);

        if (!isPhaseSecond && currentHealth <= phaseTwoThreshold)
        {
            EnterPhaseSecond();
        }

        if(!isPhaseThird && currentHealth <= phaseThreeThreshold)
        {
            EnterPhaseThird();
        }

        if (bossStateAI != null)
        {
            bossStateAI.BossFacePhase();
        }
    }

    protected override void Die()
    {
        //base.Die();
        if(bossHealthUI != null)
        {
            bossHealthUI.DestroyHealthBar();
        }
        BossFace.Instance.ChangeFace(8);
        Ending.Instance.StartDissolve();
        StartCoroutine(PlayExplosions());

        //SceneManager.LoadScene("GameClear");
    }

    private IEnumerator PlayExplosions()
    {
        GameObject[] shuffledExplosions = (GameObject[])explosionObject.Clone();
        System.Random rng = new System.Random();

        // Fisher-Yates シャッフルでランダム化
        for (int i = shuffledExplosions.Length - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (shuffledExplosions[i], shuffledExplosions[j]) = (shuffledExplosions[j], shuffledExplosions[i]);
        }

        foreach (GameObject explosion in shuffledExplosions)
        {
            explosion.SetActive(true);
            AudioManager.GetInstance().PlaySound(18);
            StartCoroutine(DeactivateAfterDelay(explosion, 1f)); // 1秒後に非実体化
            yield return new WaitForSeconds(0.5f); // 次のオブジェクトを0.5秒後に実体化
        }

        yield return new WaitForSeconds(0.5f); // 最後の爆発の処理待機
        SceneManager.LoadScene("GameClear");
    }

    private IEnumerator DeactivateAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(false);
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
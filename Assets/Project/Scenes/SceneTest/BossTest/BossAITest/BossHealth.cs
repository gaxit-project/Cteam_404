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

    [Header("最終爆発エフェクト")]
    [SerializeField] private ParticleSystem lastExplosion;
    public bool isPhaseSecond = false;
    public bool isPhaseThird = false;
    private BossStateAI bossStateAI;
    private PlayerHealth _pHealth;

    protected override void Start()
    {
        base.Start();
        bossStateAI =  GetComponent<BossStateAI>();
        _pHealth = GameObject.Find("Player").GetComponent<PlayerHealth>();
        lastExplosion.Stop();
        isPhaseSecond = false;
        isPhaseThird = false;
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
        _pHealth.SetIsInvincible();//無敵化
        BossFace.Instance.ChangeFace(8);
        //Ending.Instance.StartDissolve();
        StartCoroutine(PlayExplosions());

        //SceneManager.LoadScene("GameClear");
    }

    private IEnumerator PlayExplosions()
    {
        GameObject[] shuffledExplosions = (GameObject[])explosionObject.Clone();
        System.Random rng = new System.Random();
        BossFace.Instance.ChangeFace(8);

        // Fisher-Yates シャッフルでランダム化
        for (int i = explosionObject.Length - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (explosionObject[i], explosionObject[j]) = (explosionObject[j], explosionObject[i]);
        }

        int triggerPoint = (int)(explosionObject.Length * 0.98f); // 3/4 のタイミングを計算

        for (int i = 0; i < explosionObject.Length; i++)
        {
            explosionObject[i].SetActive(true);
            AudioManager.GetInstance().PlaySound(18);
            StartCoroutine(DeactivateAfterDelay(explosionObject[i], 1f));

            if (i == triggerPoint) // 3/4 のタイミングで実行
            {
                Ending.Instance.StartDissolve();
            }

            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitForSeconds(0.5f); // 最後の爆発の処理待機
        SceneManager.LoadScene("GameClear");
    }

    private IEnumerator DeactivateAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        BossFace.Instance.ChangeFace(8);
        //obj.SetActive(false);
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
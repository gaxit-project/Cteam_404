using UnityEngine;
using System.Collections;

public class BossSpecialAttack : MonoBehaviour
{
    [SerializeField] private ParticleSystem specialEffect;
    [SerializeField] private Collider attackCollider;

    private BossStateAI bossAI;

    private void Start()
    {
        bossAI = FindObjectOfType<BossStateAI>();
        if (attackCollider != null)
        {
            attackCollider.enabled = false;
        }
    }

    public void ExecuteAttack()
    {
        if (specialEffect != null)
        {
            specialEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            specialEffect.Play();

            AudioManager.GetInstance().PlaySound(13);

            if (attackCollider != null)
            {
                attackCollider.enabled = true;//攻撃中にコライダーを有効化
            }

            StartCoroutine(StopSpecialEffect(3f));//パーティクルとコライダーを無効化
        }
    }

    private IEnumerator StopSpecialEffect(float duration)
    {
        yield return new WaitForSeconds(duration);

        if (specialEffect != null)
        {
            specialEffect.Stop();
        }

        if (attackCollider != null)
        {
            attackCollider.enabled = false;
        }

        if (bossAI != null)
        {
            bossAI.SpecialAttackFinished();
        }
    }

    // パーティクルのコライダーにプレイヤーが触れたときの処理
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage();
            }
        }
    }
}
using UnityEngine;
using System.Collections;

public class BossSpecialAttack : MonoBehaviour
{
    [SerializeField] private ParticleSystem specialEffect; // 必殺技のパーティクルエフェクト
    [SerializeField] private Collider attackCollider;
    [SerializeField] private AudioSource specialAttackSound;
    private BossStateAI bossAI;

    private void Start()
    {
        bossAI = FindObjectOfType<BossStateAI>();
    }

    public void ExecuteAttack()
    {
        // 溜めが終わった後、必殺技発動
        if (specialEffect != null)
        {
            specialEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            specialEffect.Play();
            Debug.Log("必殺技発動！");

            if(specialAttackSound != null)
            {
                specialAttackSound.Play();
            }

            if (attackCollider != null)
            {
                attackCollider.enabled = true;
            }

            Invoke(nameof(StopSpecialEffect), 3f);
        }
        else
        {
            Debug.LogWarning("必殺技エフェクトがアタッチされていません！");
        }
    }

    private void StopSpecialEffect()
    {
        if(specialEffect != null)
        {
            specialEffect.Stop();
            Debug.Log("必殺技終了");
        }

        if(attackCollider != null)
        {
            attackCollider.enabled = false;
        }

        if(specialAttackSound != null)
        {
            specialAttackSound.Stop();
        }

        bossAI.SpecialAttackFinished();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("プレイヤーに当たった");
        }
    }
}

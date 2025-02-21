using UnityEngine;
using System.Collections;

public class BossSpecialAttack : MonoBehaviour
{
    [SerializeField] private ParticleSystem specialEffect;        // パーティクルエフェクト
    [SerializeField] private Collider attackCollider;             // パーティクルのコライダー
    [SerializeField] private AudioSource specialAttackSound;      // 特別攻撃の音
    [SerializeField] private int damageAmount = 10;               // ダメージ量（PlayerHealthに渡す値は不要）

    private BossStateAI bossAI;

    private void Start()
    {
        bossAI = FindObjectOfType<BossStateAI>();
        if (attackCollider != null)
        {
            attackCollider.enabled = false; // 初期状態でコライダーを無効
        }
    }

    public void ExecuteAttack()
    {
        if (specialEffect != null)
        {
            specialEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            specialEffect.Play();

            if (specialAttackSound != null)
            {
                specialAttackSound.Play();
            }

            if (attackCollider != null)
            {
                attackCollider.enabled = true; // 攻撃中にコライダーを有効化
            }

            StartCoroutine(StopSpecialEffect(3f)); // 3秒後にパーティクルとコライダーを無効化
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
            attackCollider.enabled = false; // コライダーを無効化
        }

        if (specialAttackSound != null)
        {
            specialAttackSound.Stop();
        }

        if (bossAI != null)
        {
            bossAI.SpecialAttackFinished();
        }
    }

    // パーティクルのコライダーにプレイヤーが触れたときの処理
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"OnTriggerEnter が呼ばれました: {other.gameObject.name}, Tag: {other.tag}, Position: {other.transform.position}, Collider: {other.GetType().Name}, Is Trigger: {other.isTrigger}");

        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage();
                Debug.Log("Playerがパーティクルダメージを受けた");
            }
            else
            {
                Debug.LogError("PlayerHealth component not found on player!");
            }
        }
    }
}
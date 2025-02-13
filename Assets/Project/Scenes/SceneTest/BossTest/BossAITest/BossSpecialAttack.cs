using UnityEngine;

public class BossSpecialAttack : MonoBehaviour
{
    [SerializeField] private ParticleSystem specialEffect; // 必殺技のパーティクルエフェクト
    [SerializeField] private Collider attackCollider;

    private void Update()
    {
        // Kキーが押されたら必殺技発動
        if (Input.GetKeyDown(KeyCode.K))
        {
            ExecuteAttack();
        }
    }

    public void ExecuteAttack()
    {
        if (specialEffect != null)
        {
            specialEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            specialEffect.Play(); // パーティクルを再生
            Debug.Log("必殺技発動！");

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
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("プレイヤーに当たった");
        }
    }
}

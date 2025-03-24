using UnityEngine;
using System.Collections;

public class BossSpecialAttack : MonoBehaviour
{
    [SerializeField] private ParticleSystem specialEffect;
    [SerializeField] private Collider attackCollider;
    [SerializeField] private Transform bossObject;
    [SerializeField] private Transform firePoint;

    [Header("攻撃位置オフセット")]
    [SerializeField] private float forwardOffset = 5f;//ボスの前方の距離
    [SerializeField] private float sideOffset = 0f;//正：右, 負：左）

    private BossStateAI bossStateAI;
    private bool isAttacking = false; // 攻撃中かどうかを判定

    private void Start()
    {
        bossStateAI = FindObjectOfType<BossStateAI>();
        if (attackCollider != null)
        {
            attackCollider.enabled = false;
        }
    }

    private void Update()
    {
        /*if (isAttacking)
        {
            Debug.Log("IsAttacking作動中");
            UpdateAttackPosition();
        }*/
    }

    public void ExecuteAttack()
    {   
        isAttacking = true;
        if (specialEffect != null)
        {
            specialEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            specialEffect.Play();

            AudioManager.GetInstance().PlaySound(14);

            if (attackCollider != null)
            {
                attackCollider.enabled = true;
            }

            
            BossFace.Instance.ChangeFace(4);
            StartCoroutine(StopSpecialEffect(3f));
            bossStateAI.BossFacePhase();
        }
    }

    private void UpdateAttackPosition()
    {
        Vector3 attackPos = GetAttackPosition();

        //firePoint の方向を攻撃の向きに設定
        Quaternion attackRotation = firePoint.rotation;

        //攻撃の位置と向きをリアルタイム更新
        if (attackCollider != null)
        {
            attackCollider.transform.position = attackPos;
            attackCollider.transform.rotation = attackRotation;
        }
    }

    private Vector3 GetAttackPosition()
    {
        Vector3 forward = bossObject.forward;
        Vector3 right = bossObject.right;

        Vector3 attackPos = bossObject.position + (forward * forwardOffset) + (right * sideOffset);
        return attackPos;
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

        isAttacking = false;

        if (bossStateAI != null)
        {
            bossStateAI.SpecialAttackFinished();
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

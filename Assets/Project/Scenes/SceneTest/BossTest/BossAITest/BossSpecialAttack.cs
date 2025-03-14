using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.UIElements;

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
    private Rigidbody attackRigidBody;
    private bool isAttacking = false; // 攻撃中かどうかを判定

    [Header("トンネリング対策")]
    [SerializeField] private float _colliderScaleFactor = 1.2f;
    [SerializeField] private float maxSpeed = 10f;
    private void Start()
    {
        bossStateAI = FindObjectOfType<BossStateAI>();
        if (attackCollider != null)
        {
            attackCollider.enabled = false;

            attackRigidBody = attackCollider.GetComponent<Rigidbody>();
            if(attackRigidBody == null)
            {
                attackRigidBody = attackCollider.gameObject.AddComponent<Rigidbody>();
            }

            attackRigidBody.isKinematic = false;
            attackRigidBody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }

        AdjustColiderSize();
    }

    private void Update()
    {
        if (isAttacking)
        {
            UpdateAttackPosition();
        }
    }

    public void ExecuteAttack()
    {
        if (specialEffect != null)
        {
            specialEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            specialEffect.Play();

            AudioManager.GetInstance().PlaySound(14);

            if (attackCollider != null)
            {
                attackCollider.enabled = true;
            }

            isAttacking = true;
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

        //RigidBodyの速度を制限
        if(attackRigidBody != null)
        {
            Vector3 velocity = (attackPos - attackCollider.transform.position) / Time.deltaTime;
            attackRigidBody.linearVelocity = Vector3.ClampMagnitude(velocity, maxSpeed);
        }
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

        Vector3 attackPos = bossObject.position+ (forward * forwardOffset) + (right * sideOffset);
        attackPos.y -= 20f;
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
    /// <summary>
    /// コライダーのサイズを調整して、トンネリングを防ぐ
    /// </summary>
    private void AdjustColiderSize()
    {
        BoxCollider boxCollider = attackCollider as BoxCollider;
        if(boxCollider != null)
        {
            boxCollider.size *= _colliderScaleFactor;
        }
    }   
    // パーティクルのコライダーにプレイヤーが触れたときの処理
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Triggerでダメージ");
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage();
                
            }
            else
            {
                Debug.Log("PlayerHealthがないよ");
            }
        }
        else
        {
            Debug.Log("PlayerTagがないよ");
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage();
                Debug.Log("Collitionでダメージ");
            }
            else
            {
                Debug.Log("PlayerHealthがないよ");
            }
        }
        else
        {
            Debug.Log("PlayerTagがないよ");
        }
    }


}

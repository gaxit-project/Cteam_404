using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
using UnityEngine.Audio;

public class PlayerController : MonoBehaviour
{

    public float Speed;

    public float RotateSpeed;

    private Vector3 MoveValue;

    private bool isAttacking = false; // 攻撃中かどうかのフラグ
    private bool canRide = false; // 攻撃中かどうかのフラグ

    // アニメーターコンポーネント
    private Animator animator;

    Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

    }

    void Update()
    {
        // Enterキーで攻撃
        if (Input.GetKeyDown(KeyCode.Return) && !isAttacking && !canRide)
        {
            if(!isAttacking && !canRide)
            {
                Attack();
            }
            else if(!isAttacking && canRide)
            {
                PlayerPrefs.SetInt("isRide", 1);
            }
            
        }
    }

    void FixedUpdate()
    {


        animator.SetFloat("speed", MoveValue.magnitude);
        Debug.Log("速度" +  MoveValue.magnitude);

        MoveValue = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical")).normalized;

        rb.linearVelocity = MoveValue * Speed;

        // 入力がある場合のみ回転を更新
        if (MoveValue.magnitude > 0)
        {
            // キャラクターを移動方向に回転
            Quaternion targetRotation = Quaternion.LookRotation(MoveValue);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, Time.deltaTime * RotateSpeed);
        }
    }

    void Attack()
    {
        // 攻撃フラグを立てる
        isAttacking = true;

        // 攻撃アニメーションを再生
        animator.SetBool("isAttack", isAttacking);

        // Debug.Logで攻撃を出力
        Debug.Log("攻撃!");
    }

    // アニメーションイベントまたは遅延処理で攻撃終了を検知
    public void EndAttack()
    {
        isAttacking = false; // 攻撃終了を許可
        animator.SetBool("isAttack", isAttacking);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("RailArea"))
        {
            canRide = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("RailArea"))
        {
            canRide = false;
        }
    }
}

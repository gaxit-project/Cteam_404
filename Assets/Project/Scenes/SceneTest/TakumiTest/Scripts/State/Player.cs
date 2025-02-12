using UnityEngine;
using SplineMesh;
using UnityEngine.UIElements;
using Unity.VisualScripting;

public partial class  Player : MonoBehaviour
{
    public Spline CurrentRail;
    public float Speed = 5f;
    public float JumpHeight = 2f;
    public float JumpDuration = 0.5f;
    public float RotateSpeed;
    [Header("レールへの吸着が発生する距離")]
    public float _snapDistance = 1.5f; // 吸着が有効となる距離

    private Rigidbody rb;
    private Animator animator;
    private PlayerStateBase currentState;

    public Vector3 po = Vector3.zero;

    public float _railPosition = 0f;       // レール上の現在位置 (0〜1で表現)
    private bool _isJumping = false;        // ジャンプ中かどうかのフラグ
    private bool _leftPosition = false;     // 左側にレールがあるか
    private bool _rightPosition = false;    // 右側にレールがあるか
    private Vector3 left;
    private Vector3 right;
    private Spline _leftRail = null;        // 左側のレール
    private Spline _rightRail = null;       // 右側のレール
    private float _leftRailPosition = 0f;   // 左レールの位置 (0〜1で表現)
    private float _rightRailPosition = 0f;  // 右レールの位置 (0〜1で表現)
    private bool canFall = false;

    public bool isRide = true;


    public bool isAttacking = false; // 攻撃中かどうかのフラグ
    public bool canRide = false; // 攻撃中かどうかのフラグ



    private static readonly StateRailMove stateRailMove = new StateRailMove();
    //private static readonly StateJump stateJump = new StateJump(null, 0f, );
    private static readonly StateAttack stateAttack = new StateAttack();


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        ChangeState(stateRailMove);
    }

    private void Update()
    {
        currentState.OnUpdate(this);
        Debug.Log("現在の状態 : " +  currentState);
    }

    public void ChangeState(PlayerStateBase newState)
    {
        currentState?.OnExit(this, newState);
        newState.OnEnter(this, currentState);
        currentState = newState;
    }
}
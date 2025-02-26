using UnityEngine;
using SplineMesh;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine.InputSystem;

public partial class  Player : MonoBehaviour
{
    [Header("現在のレール")]
    public  Spline CurrentRail;
    [Header("レール上スピード")]
    public float Speed = 10f;
    private float MinSpeed;
    private float MaxSpeed;
    [Header("ジャンプ高さ")]
    public float JumpHeight = 2f;
    [Header("ジャンプ時間")]
    public float JumpDuration = 0.5f;
    [Header("レールへの吸着が発生する距離")]
    public float _snapDistance = 8f; // 吸着が有効となる距離
    [Header("武器")]
    public GameObject arms;

    protected Camera mainCamera;
    protected PlayerEmpty _playerEmpty;

    protected float _railPosition = 0f;       // レール上の現在位置 (0〜1で表現)
    public bool _leftPosition = false;     // 左側にレールがあるか
    public bool _rightPosition = false;    // 右側にレールがあるか
    private Vector3 left;
    private Vector3 right;
    private Spline _leftRail = null;        // 左側のレール
    private Spline _rightRail = null;       // 右側のレール
    private float _leftRailPosition = 0f;   // 左レールの位置 (0〜1で表現)
    private float _rightRailPosition = 0f;  // 右レールの位置 (0〜1で表現)

    #region ULT
    [Header("ビームチャージに必要なモブ数")]
    [SerializeField]
    public int _attackMob = 5; //ビームを発射するために必要なモブの撃破数

    [Header("ULT時間")]
    [SerializeField]
    public float _ultTime = 5f; //ビームの発射時間

    [Header("ULT攻撃力")]
    [SerializeField]
    protected int _damegeULT = 50; //ULTの攻撃

    [Header("ボス")]
    [SerializeField]
    protected GameObject _boss;

    public int _mobCounter = 0;      // 倒したモブの数
    public int prevMobCounter = 0;

    protected bool canULT = false;
    public bool isULT = false;
    private ParticleSystem particle; // ビームのエフェクト用のパーティクルシステム
    private SliderPlayerBeam sliderPlayerBeam;　//ビームチャージ用スライダーの管理スクリプト
    #endregion

    protected bool canFall = false;
    protected bool isRide = true;
    protected bool isAttacking = false; // 攻撃中かどうかのフラグ
    protected bool canRide = false; // レールに乗れるかどうかのフラグ
    protected bool isJumping = false;

    #region ステート
    protected PlayerStateBase currentState;
    private static readonly StateRailMove stateRailMove = new StateRailMove();
    private static readonly StateAttack stateAttack = new StateAttack();
    private static readonly StateULTAttack stateUltAttack = new StateULTAttack();
    #endregion

    private Rigidbody rb;
    private Animator animator;




    [SerializeField] private InputActionReference _hold;
    [SerializeField] private Image _gaugeImage;

    private InputAction _holdAction;
    public static bool gaugeStatus = false;

    public static float progress;
    public static bool gaugeActivated = false;

    float lockTime = 0;

    public void OnAttack(InputAction.CallbackContext context)
    {

        // Performedフェーズの判定を行う
        if (context.phase == InputActionPhase.Performed)
        {
            isAttacking = true;
        }
    }


    private void Awake()
    {
        if (_hold == null) return;

        _holdAction = _hold.action;
        _holdAction.Enable();
    }

    private void Start()
    {
        //パーティクルシステムを取得
        particle = GetComponentInChildren<ParticleSystem>();
        particle.Stop();
        arms.SetActive(false);

        //スライダーUIを管理するスクリプトを取得
        //sliderPlayerBeam = GetComponentInChildren<SliderPlayerBeam>();
        _mobCounter = 0;

        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        mainCamera = Camera.main;
        _playerEmpty = GameObject.Find("PlayerEmpty").GetComponent<PlayerEmpty>();

        AudioManager.GetInstance().PlayBGM(2);

        MinSpeed = Speed * 0.25f;
        MaxSpeed = Speed * 1.5f;

        ChangeState(stateRailMove);
    }

    private void Update()
    {
        currentState.OnUpdate(this);
        Debug.Log("現在の状態 : " +  currentState);
        Debug.Log("レール差分" + (_railPosition - _playerEmpty._railPosition));

        //　モブ撃破数が必要数に達した場合、Enterキーでビームを発射できる
        if (_mobCounter >= _attackMob)
        {
            canULT = true;
        }

    }

    public void ChangeState(PlayerStateBase newState)
    {
        currentState?.OnExit(this, newState);
        newState.OnEnter(this, currentState);
        currentState = newState;
    }

    /// <summary>
    /// モブにヒットした回数を加算するメソッド
    /// </summary>
    public void AddMobHit()
    {
        if (_mobCounter < _attackMob)
        {
            prevMobCounter = _mobCounter;
            _mobCounter++;//撃破数を増やす
        }
        Debug.Log("モブヒット回数: " + _mobCounter);
    }
}
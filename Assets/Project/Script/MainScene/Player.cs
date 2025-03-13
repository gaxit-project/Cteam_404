using UnityEngine;
using SplineMesh;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

public partial class  Player : MonoBehaviour
{
    //プレイヤー設定
    #region レール移動
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
    public float _snapDistance = 15f; // 吸着が有効となる距離
    #endregion

    #region 通常攻撃

    [Header("武器")]
    public GameObject arms;
    [Header("攻撃エフェクト")]
    public ParticleSystem Slash;

    #endregion

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

    #region ULT攻撃
    [Header("ビームチャージに必要なモブ数")]
    [SerializeField]
    public int _attackMob = 5; //ビームを発射するために必要なモブの撃破数

    [Header("ULT待機エフェクト")]
    [SerializeField]
    protected ParticleSystem UltStay; //ビームを発射待機状態
    protected ParticleSystem.MainModule main;
    protected ParticleSystem.EmissionModule emission;

    [Header("ビームエフェクト")]
    [SerializeField]
    protected ParticleSystem particle; // ビームのエフェクト用のパーティクルシステム

    [Header("ULT時間")]
    [SerializeField]
    public float _ultTime = 5f; //ビームの発射時間

    [Header("ULTたまる時間")]
    [SerializeField]
    protected float UltChargeTime = 25f; // ULTがたまる時間

    [Header("ULT攻撃力")]
    [SerializeField]
    protected int _damegeULT = 50; //ULTの攻撃

    [Header("ボス")]
    [SerializeField]
    protected GameObject _boss;
    [Header("ボスセンター")]
    [SerializeField]
    protected GameObject _bossCenter;

    private BossHealth bossHealth;

    public int _mobCounter = 0;      // 倒したモブの数

    public static float UltGauge = 0;

    protected bool canULT = false;
    public bool isULT = false;
    
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

    #region InputAction


    [SerializeField] private InputActionReference _hold;
    [SerializeField] private Image _gaugeImage;

    private InputAction _holdAction;
    public static bool gaugeStatus = false;

    public static float progress;
    public static bool gaugeActivated = false;

    float lockTime = 0;

    #endregion


    private void Awake()
    {
        if (_hold == null) return;

        _holdAction = _hold.action;
        _holdAction.Enable();

        // イベント登録
        _holdAction.performed += OnHoldPerformed;
        _holdAction.canceled += OnHoldCanceled;
    }

    private void Start()
    {
        // コンポーネント取得
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        mainCamera = Camera.main;
        _playerEmpty = GameObject.Find("PlayerEmpty").GetComponent<PlayerEmpty>();
        bossHealth = _boss.GetComponent<BossHealth>();

        main = UltStay.main;
        emission = UltStay.emission;

        InitProperties();

        AudioManager.GetInstance().PlayBGM(2);

        ChangeState(stateRailMove);
    }

    private void Update()
    {
        if (bossHealth.isPhaseSecond && !bossHealth.isPhaseThird)
        {
            Speed = 13f;
            MinSpeed = Speed * 0.25f;
            MaxSpeed = Speed * 1.5f;
        }
        else if(!bossHealth.isPhaseSecond && bossHealth.isPhaseThird)
        {
            Speed = 17f;
            MinSpeed = Speed * 0.25f;
            MaxSpeed = Speed * 1.5f;
        }
        else
        {
            Speed = 10f;
            MinSpeed = Speed * 0.25f;
            MaxSpeed = Speed * 1.5f;
        }
        currentState.OnUpdate(this);
        //Debug.Log("現在の状態 : " +  currentState);

        //　モブ撃破数が必要数に達した場合、Enterキーでビームを発射できる

        if (Input.GetKeyDown("n"))
        {
            AddMobHit();
            UltStay.Play();
        }

        UltGaugeVolume();



    }

    public void ChangeState(PlayerStateBase newState)
    {
        currentState?.OnExit(this, newState);
        newState.OnEnter(this, currentState);
        currentState = newState;
    }

    #region InputSystem_Callback
    public void OnAttack(InputAction.CallbackContext context)
    {

        // Performedフェーズの判定を行う
        if (context.phase == InputActionPhase.Performed)
        {
            isAttacking = true;
        }
    }
    #endregion

    /// <summary>
    /// モブにヒットした回数を加算するメソッド
    /// </summary>
    public void AddMobHit()
    {
        if (_mobCounter < _attackMob)
        {
            _mobCounter++;//撃破数を増やす
            UltGauge = Mathf.Clamp01(UltGauge += 1.0f / _attackMob);
        }
        Debug.Log("モブヒット回数: " + _mobCounter);
    }

    public void UltGaugeVolume()
    {
        if (currentState != stateUltAttack && UltGauge <= 1f)
        {
            UltGauge += Time.deltaTime / UltChargeTime;
        }
        UltGauge = Mathf.Clamp01(UltGauge);
        if (UltGauge >= 1f)
        {
            canULT = true;
            UltGauge = 1f;
            main.startSpeed = 1f;
        }
        emission.rateOverTime = UltGauge * 10f;
    }

    #region 初期化
    public void InitProperties()
    {
        canULT = false;
        isULT = false;
        canFall = false;
        isRide = true;
        isAttacking = false;
        canRide = false;
        isJumping = false;

        _railPosition = 0f;

        _mobCounter = 0;
        UltGauge = 0f;

        main.startSpeed = 0f;
        emission.rateOverTime = 0f;

        particle.Stop();
        arms.SetActive(false);
    }
    #endregion
}
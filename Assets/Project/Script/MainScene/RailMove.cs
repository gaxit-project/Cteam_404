using UnityEngine;
using SplineMesh;

public class RailMove : MonoBehaviour
{
    /// <summary>
    /// レール移動を制御するクラス。
    /// SplineMeshライブラリを利用してキャラクターをレールに沿って移動させる。
    /// </summary>

    [Header("ジャンプの高さ")]
    [SerializeField] private float _jumpHeight = 2f;
    [Header("ジャンプ時間")]
    [SerializeField] private float _jumpDuration = 0.5f;
    [Header("レールへの吸着が発生する距離")]
    [SerializeField] private float _snapDistance = 1.5f;
    [Header("レール上の移動速度")]
    public float _speed = 5f;


    [Header("現在のレール")]
    public Spline CurrentRail;
    private float _railPosition = 0f;       // レール上の現在位置 (0〜1の範囲で表現)
    private bool _isJumping = false;        // ジャンプ中かどうかのフラグ

    [Header("接続可能レールリスト")]
    public Spline[] ConnectedRails;

    private Vector3 _jumpTarget;            // ジャンプ先のターゲット位置
    private Spline _snappedRail;            // 吸着したレール

    void Update()
    {
        // ジャンプ中は移動処理をスキップ
        if (_isJumping) return;

        // レールに沿った移動処理
        _railPosition += _speed * Time.deltaTime / CurrentRail.Length;

        // レールの終点に達した場合、移動をリセット
        if (_railPosition >= 0.99f)
        {
            _railPosition = 0f;
            return;
        }

        MoveAlongRail();

        // 左右入力でジャンプ処理を呼び出し
        if (Input.GetKeyDown("a"))
        {
            Jump(-1); // 左方向にジャンプ
        }
        else if (Input.GetKeyDown("d"))
        {
            Jump(1); // 右方向にジャンプ
        }
    }

    /// <summary>
    /// レール上の現在の位置と向きを更新
    /// </summary>
    void MoveAlongRail()
    {
        // 現在のレール上の位置情報を取得
        var splineSample = CurrentRail.GetSampleAtDistance(_railPosition * CurrentRail.Length);
        transform.position = splineSample.location;   // レール上の位置を設定
        transform.forward = splineSample.tangent;     // レールの進行方向に向きを設定
    }

    /// <summary>
    /// レールジャンプ
    /// </summary>
    void Jump(int direction)
    {
        // 接続されたレールがない場合、ジャンプをスキップ
        if (ConnectedRails.Length == 0) return;

        _isJumping = true; // ジャンプ中フラグを立てる
        _jumpTarget = CalculateJumpTarget(direction); // ジャンプ先を計算
        StartCoroutine(JumpToRail());
    }

    /// <summary>
    /// ジャンプ先に位置計算
    /// </summary>
    Vector3 CalculateJumpTarget(int direction)
    {
        Vector3 jumpOffset = transform.right * direction * 2f; // 左右方向にオフセットを設定
        return transform.position + jumpOffset;
    }

    /// <summary>
    /// ジャンプ中の移動計算
    /// </summary>
    private System.Collections.IEnumerator JumpToRail()
    {
        float elapsedTime = 0f;    // 経過時間
        Vector3 startPosition = transform.position; // ジャンプ開始時の位置

        while (elapsedTime < _jumpDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / _jumpDuration; // 経過時間の割合

            // 放物線を表現するために高さを加算
            float height = Mathf.Sin(t * Mathf.PI) * _jumpHeight;
            transform.position = Vector3.Lerp(startPosition, _jumpTarget, t) + Vector3.up * height;

            // レールへの吸着判定を実行
            CheckForSnap();

            yield return null;
        }

        // 吸着していない場合は着地処理を実行
        if (_snappedRail == null)
        {
            OnLand();
        }
        else
        {
            SnapToRail(_snappedRail); // 吸着したレールにスナップ
        }
    }


    /// <summary>
    /// ジャンプ中に他のレールに吸着可能かどうかを判定
    /// </summary>
    void CheckForSnap()
    {
        foreach (var rail in ConnectedRails)
        {
            if (rail == CurrentRail) continue;

            // 高さを無視して水平位置を投影
            Vector3 horizontalPosition = new Vector3(transform.position.x, rail.GetSampleAtDistance(0).location.y, transform.position.z);
            var projection = rail.GetProjectionSample(horizontalPosition);

            float distance = Vector3.Distance(transform.position, projection.location);

            if (distance < _snapDistance)
            {
                _snappedRail = rail;
                break;
            }
        }
    }


    /// <summary>
    /// ジャンプが終了し、吸着に失敗した場合の着地処理
    /// </summary>

    /*
    void OnLand()
    {
        // 着地時に最も近いレールを探す
        Spline closestRail = FindClosestRail();

        if (closestRail != null)
        {
            var projection = closestRail.GetProjectionSample(transform.position);
            CurrentRail = closestRail;
            _railPosition = projection.distanceInCurve / closestRail.Length; // レール上の位置を更新
        }

        _isJumping = false; // ジャンプ終了フラグをリセット
        _snappedRail = null; // 吸着したレールをリセット
    }*/

    void OnLand()
    {
        // RailManagerを取得
        RailManager railManager = CurrentRail.GetComponent<RailManager>();
        if (railManager != null)
        {
            // 最寄りの参照用オブジェクトを取得
            Transform closestPoint = railManager.GetClosestReferencePoint(transform.position);
            if (closestPoint != null)
            {
                // レール位置を最寄りポイントに基づいて更新
                transform.position = closestPoint.position;

                var projection = CurrentRail.GetProjectionSample(closestPoint.position);
                _railPosition = projection.distanceInCurve / CurrentRail.Length;
            }
        }
        else
        {
            Debug.LogError("RailManagerが現在のレールにアタッチされていません。");
        }

        _isJumping = false; // ジャンプ終了フラグをリセット
        _snappedRail = null; // 吸着したレールをリセット
    }



    /// <summary>
    /// 現在位置に最も近いレールを検索
    /// </summary>
    Spline FindClosestRail()
    {
        Spline closestRail = null;
        float closestDistance = float.MaxValue;

        foreach (var rail in ConnectedRails)
        {
            var projection = rail.GetProjectionSample(transform.position);
            float distance = Vector3.Distance(transform.position, projection.location);

            if (distance < closestDistance)
            {
                closestRail = rail;
                closestDistance = distance;
            }
        }

        return closestRail;
    }



    /// <summary>
    /// 他のレールへの吸着処理
    /// </summary>
    void SnapToRail(Spline targetRail)
    {
        var projection = targetRail.GetProjectionSample(transform.position);
        CurrentRail = targetRail;

        // 投影された位置をそのまま使用
        _railPosition = projection.distanceInCurve / targetRail.Length;

        transform.position = projection.location;
        transform.forward = projection.tangent;

        _isJumping = false;
        _snappedRail = null;
    }

    /// <summary>
    /// DebugLogのメソッドです
    /// </summary>
    void DebugLog(string message)
    {
#if UNITY_EDITOR
        Debug.Log(message);
#endif
    }
}

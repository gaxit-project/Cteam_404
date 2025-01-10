using UnityEngine;
using SplineMesh;
using System.Linq;

public class RailMove : MonoBehaviour
{
    #region 
    [Header("ジャンプの高さ")]
    [SerializeField] private float _jumpHeight = 2f; // ジャンプの最大高さ
    [Header("ジャンプ時間")]
    [SerializeField] private float _jumpDuration = 0.5f; // ジャンプの所要時間
    [Header("レールへの吸着が発生する距離")]
    [SerializeField] private float _snapDistance = 1.5f; // 吸着が有効となる距離
    [Header("レール上の移動速度")]
    public float _speed = 5f; // レール上の移動速度
    [Header("現在のレール")]
    public Spline CurrentRail; // 現在のレール
    #endregion

    private float _railPosition = 0f;       // レール上の現在位置 (0〜1で表現)
    private bool _isJumping = false;        // ジャンプ中かどうかのフラグ
    private bool _leftPosition = false;     // 左側にレールがあるか
    private bool _rightPosition = false;    // 右側にレールがあるか
    private Spline _leftRail = null;        // 左側のレール
    private Spline _rightRail = null;       // 右側のレール
    private float _leftRailPosition = 0f;   // 左レールの位置 (0〜1で表現)
    private float _rightRailPosition = 0f;  // 右レールの位置 (0〜1で表現)

    void Update()
    {
        try
        {
            if (_isJumping) return; // ジャンプ中は他の処理をスキップ

            // レールに沿った移動処理
            _railPosition += _speed * Time.deltaTime / CurrentRail.Length;
            if (_railPosition >= 0.99f)
            {
                _railPosition = 0f; // レールをループする場合
            }

            MoveAlongRail(); // レール上の移動
            UpdateReferencePositions(); // 左右のレール位置を更新

            // 左右入力でジャンプ処理
            if (Input.GetKeyDown("a") && _leftPosition)
            {
                JumpToRail(_leftRail, _leftRailPosition);
            }
            else if (Input.GetKeyDown("d") && _rightPosition)
            {
                JumpToRail(_rightRail, _rightRailPosition);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Update内で例外が発生しました: " + ex.Message);
        }
    }

    /// <summary>
    /// レール上の現在の位置と向きを更新
    /// </summary>
    #region レール上の現在の位置と向きを更新
    void MoveAlongRail()
    {
        try
        {
            var splineSample = CurrentRail.GetSampleAtDistance(_railPosition * CurrentRail.Length);
            transform.position = splineSample.location;
            transform.forward = splineSample.tangent;
        }
        catch (System.Exception ex)
        {
            Debug.LogError("MoveAlongRailで例外が発生しました: " + ex.Message);
        }
    }
    #endregion

    /// <summary>
    /// 他のレールの参照用オブジェクトを調査して左右のレールポジションを更新
    /// </summary>
    #region 他のレールの参照用オブジェクトを調査して左右のレールポジションを更新
    void UpdateReferencePositions()
    {
        try
        {
            RailManager[] railManagers = FindObjectsOfType<RailManager>();

            // 状態をリセット
            _leftPosition = false;
            _rightPosition = false;
            _leftRail = null;
            _rightRail = null;

            foreach (var manager in railManagers)
            {
                // 現在のレールはスキップ
                if (manager.TargetRail == CurrentRail) continue;

                int closestIndex = manager.GetNearPositionIndex(transform.position);
                if (closestIndex == -1) continue; // 有効な参照がない場合スキップ

                for (int i = 0; i < manager.ReferenceObjects.Length; i++)
                {
                    Vector3 referenceObject = manager.GetNearPosition(i);
                    float distance = Vector3.Distance(transform.position, referenceObject);

                    if (distance > _snapDistance) continue; // スナップ距離外の場合スキップ

                    Vector3 toObject = referenceObject - transform.position;
                    float dot = Vector3.Dot(Vector3.right, toObject.normalized);

                    if (dot < -0.5f && !_leftPosition) // 左側
                    {
                        _leftPosition = true;
                        _leftRail = manager.TargetRail;
                        _leftRailPosition = manager.GetNearRailPosition(i);
                    }
                    else if (dot > 0.5f && !_rightPosition) // 右側
                    {
                        _rightPosition = true;
                        _rightRail = manager.TargetRail;
                        _rightRailPosition = manager.GetNearRailPosition(i);
                    }
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("UpdateReferencePositionsで例外が発生しました: " + ex.Message);
        }
    }
    #endregion



    /// <summary>
    /// 指定したレールにジャンプする
    /// </summary>
    /// <param name="targetRail"></param>
    /// <param name="targetPosition"></param>
    #region 指定したレールにジャンプする
    void JumpToRail(Spline targetRail, float targetPosition)
    {
        try
        {
            _isJumping = true;

            Vector3 startPosition = transform.position;
            var splineSample = targetRail.GetSampleAtDistance(targetPosition * targetRail.Length);
            Vector3 endPosition = splineSample.location;

            StartCoroutine(JumpCoroutine(startPosition, endPosition, () =>
            {
                CurrentRail = targetRail;
                _railPosition = targetPosition;
                _isJumping = false;
            }));
        }
        catch (System.Exception ex)
        {
            Debug.LogError("JumpToRailで例外が発生しました: " + ex.Message);
        }
    }
    #endregion

    /// <summary>
    /// ジャンプアニメーション
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="onComplete"></param>
    /// <returns></returns>
    #region ジャンプアニメーション
    private System.Collections.IEnumerator JumpCoroutine(Vector3 start, Vector3 end, System.Action onComplete)
    {
        float elapsed = 0f;

        while (elapsed < _jumpDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / _jumpDuration;

            // ジャンプの軌跡 (放物線)
            float height = Mathf.Sin(t * Mathf.PI) * _jumpHeight;
            transform.position = Vector3.Lerp(start, end, t) + Vector3.up * height;

            yield return null;
        }

        onComplete?.Invoke();
    }
    #endregion
}

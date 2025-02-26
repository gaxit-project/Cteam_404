using UnityEngine;
using SplineMesh;
using static Player;
using static UnityEngine.UI.GridLayoutGroup;
using System.ComponentModel;
using TMPro;

public class PlayerEmpty : MonoBehaviour
{
    private Player _player;
    private float _currentSpeed;
    public float _railPosition;
    private Spline _currentRail;
    public float MinPos = 0.028f;
    public float MaxPos = 0.023f;

    private bool _leftPosition = false;     // 左側にレールがあるか
    private bool _rightPosition = false;    // 右側にレールがあるか
    private Vector3 left;
    private Vector3 right;
    private Spline _leftRail = null;        // 左側のレール
    private Spline _rightRail = null;       // 右側のレール
    public float _leftRailPosition = 0f;   // 左レールの位置 (0〜1で表現)
    public float _rightRailPosition = 0f;  // 右レールの位置 (0〜1で表現)

    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _currentRail = _player.CurrentRail;
    }

    void Update()
    {
        _currentSpeed = _player.Speed;

        MoveAlongRail();
        UpdateReferencePositions();

        #region プレイヤーレール操作

        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        float angle = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;
        if (input.magnitude > 0.1f) // 適当な閾値
        {
            if (angle >= 45 && angle < 135) // 上
            {
                if (_leftPosition && _player._leftPosition)
                {
                    JumpEmpty(_leftRail, _leftRailPosition, left);
                }
            }
            else if (angle >= -135 && angle < -45) // 下
            {
                if (_rightPosition && _player._rightPosition)
                {
                    JumpEmpty(_rightRail, _rightRailPosition, right);
                }
            }

        }

        #endregion


        _railPosition += _currentSpeed * Time.deltaTime / _currentRail.Length;
        if (_railPosition >= 0.9999f)
        {
            {
                _railPosition = 0f; // ループ処理
            }
        }
    }

    /// <summary>
    /// レール上の現在の位置と向きを更新
    /// </summary>
    #region レール上の現在の位置と向きを更新
    void MoveAlongRail()
    {
        var splineSample = _currentRail.GetSampleAtDistance(_railPosition * _currentRail.Length);
        transform.position = splineSample.location;
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
                if (manager.TargetRail == _currentRail) continue;

                int closestIndex = manager.GetNearPositionIndex(transform.position);
                if (closestIndex == -1) continue; // 有効な参照がない場合スキップ

                Vector3 referenceObject = manager.GetNearPosition(closestIndex);
                float distance = Vector3.Distance(transform.position, referenceObject);

                if (distance > _player._snapDistance) continue; // スナップ距離外の場合スキップ

                Vector3 toObject = referenceObject - transform.position;
                float dot = Vector3.Dot(transform.right, toObject.normalized);

                if (dot < -0.5f && !_leftPosition) // 左側
                {
                    _leftPosition = true;
                    _leftRail = manager.TargetRail;
                    _leftRailPosition = manager.GetNearRailPosition(closestIndex);
                    left = manager.GetNearPosition(closestIndex);
                }
                else if (dot > 0.5f && !_rightPosition) // 右側
                {
                    _rightPosition = true;
                    _rightRail = manager.TargetRail;
                    _rightRailPosition = manager.GetNearRailPosition(closestIndex);
                    right = manager.GetNearPosition(closestIndex);
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("UpdateReferencePositionsで例外が発生しました: " + ex.Message);
        }
    }
    #endregion


    void JumpEmpty(Spline targetRail, float targetPosition, Vector3 target)
    {
        _currentRail = targetRail;
        _railPosition = targetPosition;
        this.transform.position = target;
    }



}

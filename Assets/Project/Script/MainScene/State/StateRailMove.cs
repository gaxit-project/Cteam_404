using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using SplineMesh;
using static UnityEngine.UI.GridLayoutGroup;


public partial class Player
{
    /// <summary>
    /// レール上の移動ステート
    /// </summary>
    public class StateRailMove : PlayerStateBase
    {
        private float _currentSpeed;

        public override void OnEnter(Player owner, PlayerStateBase prevState)
        {
            _currentSpeed = owner.Speed;
        }

        public override void OnUpdate(Player owner)
        {

            owner.MoveAlongRail();
            owner.UpdateReferencePositions(_currentSpeed);
            owner.arms.SetActive(false);
            #region プレイヤーレール操作

            _currentSpeed = owner.Speed;

            Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            float angle = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;

            if (input.magnitude > 0.1f) // 適当な閾値
            {
                if (!owner.isAttacking) // 攻撃中はジャンプ不可
                {
                    if (angle >= 45 && angle < 135) // 上
                    {
                        if (owner._leftPosition)
                        {
                            owner.isJumping = !owner.isJumping;
                            owner.ChangeState(new StateJump(owner._leftRail, owner._leftRailPosition, owner.left));
                        }
                    }
                    else if (angle >= -135 && angle < -45) // 下
                    {
                        if (owner._rightPosition)
                        {
                            owner.isJumping = !owner.isJumping;
                            owner.ChangeState(new StateJump(owner._rightRail, owner._rightRailPosition, owner.right));
                        }
                    }
                }

                if (angle >= -45 && angle < 45) // 右
                {
                    _currentSpeed = owner.MaxSpeed;
                }
                else // 左
                {
                    _currentSpeed = owner.MinSpeed;
                }
            }

            // 攻撃
            if (!owner.isJumping) // ジャンプ中は攻撃不可
            {
                if (owner.canULT)
                {


                    if (owner._holdAction.IsPressed())
                    {
                        progress += Time.deltaTime / 1.0f; // 2秒間で100%になる
                        progress = Mathf.Clamp01(progress);
                    }


                    if (progress >= 1 && !gaugeActivated)
                    {
                        /*
                        owner._holdAction.Disable();
                        owner._holdAction.Enable();
                        progress = 0.0f;

                        Debug.Log("ULT発動");
                        owner.ChangeState(stateUltAttack);
                        */
                    }
                    else
                    {
                        if (owner.isAttacking)
                        {
                            owner.isAttacking = !owner.isAttacking;
                            owner.ChangeState(stateAttack);
                        }
                    }
                }
                else
                {
                    if (owner.isAttacking)
                    {
                        owner.isAttacking = !owner.isAttacking;
                        owner.ChangeState(stateAttack);
                    }
                }
            }

            #endregion


            if (owner._railPosition >= owner._playerEmpty._railPosition + owner._playerEmpty.MaxPos)
            {
                _currentSpeed = owner.MinSpeed;
            }
            owner._railPosition += _currentSpeed * Time.deltaTime / owner.CurrentRail.Length;
            if (owner._railPosition >= 0.9999f)
            {
                if (owner.canFall)
                {
                    owner.isRide = false;
                    owner.canFall = false;
                    owner._railPosition = 0f;
                }
                else
                {
                    owner._railPosition = 0f; // ループ処理
                }
            }
            owner._railPosition = Mathf.Clamp(owner._railPosition, owner._playerEmpty._railPosition - owner._playerEmpty.MinPos, owner._playerEmpty._railPosition + owner._playerEmpty.MaxPos);
        }
    }

    #region InputSystem_Callback
    // 長押し開始
    private void OnHoldPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("長押し開始");
        progress = 0.0f;
    }

    // 長押しを離した瞬間
    private void OnHoldCanceled(InputAction.CallbackContext context)
    {
        Debug.Log("長押しを離した瞬間");

        // 一定時間以上長押ししていたらULT発動
        if (progress >= 1.0f && canULT)
        {
            Debug.Log("ULT発動！");
            isULT = true;

            // ULT発動処理
            ChangeState(stateUltAttack);
        }

        // 長押しゲージリセット
        progress = 0.0f;
    }
    #endregion

    /// <summary>
    /// レール上の現在の位置と向きを更新
    /// </summary>
    #region レール上の現在の位置と向きを更新
    void MoveAlongRail()
    {
        if (isRide)
        {
            try
            {
                var splineSample = CurrentRail.GetSampleAtDistance(_railPosition * CurrentRail.Length);
                transform.position = splineSample.location;
                if (isULT)
                {
                    transform.LookAt(_bossCenter.transform.position);
                }
                else
                {
                    transform.forward = splineSample.tangent;
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("MoveAlongRailで例外が発生しました: " + ex.Message);
            }
        }

    }
    #endregion

    /// <summary>
    /// 他のレールの参照用オブジェクトを調査して左右のレールポジションを更新
    /// </summary>
    #region 他のレールの参照用オブジェクトを調査して左右のレールポジションを更新
    void UpdateReferencePositions(float _currentSpeed)
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

                Vector3 referenceObject = manager.GetNearPosition(closestIndex);
                float tmpRailposition = manager.GetNearRailPosition(closestIndex);
                float distance = Vector3.Distance(transform.position, referenceObject);

                if (distance > _snapDistance) continue; // スナップ距離外の場合スキップ

                Vector3 toObject = referenceObject - transform.position;
                float dot = Vector3.Dot(transform.right, toObject.normalized);

                if (dot < -0.5f && !_leftPosition) // 左側
                {
                    _leftPosition = true;
                    _leftRail = manager.TargetRail;

                    float futureRailPosition = tmpRailposition + _currentSpeed * JumpDuration / _leftRail.Length;

                    var splineSample = _leftRail.GetSampleAtDistance(futureRailPosition * _leftRail.Length);
                    Vector3 target = splineSample.location;

                    int jumpIndex = manager.GetNearPositionIndex(target);
                    _leftRailPosition = manager.GetNearRailPosition(jumpIndex);
                    left = manager.GetNearPosition(jumpIndex);
                }
                else if (dot > 0.5f && !_rightPosition) // 右側
                {
                    _rightPosition = true;
                    _rightRail = manager.TargetRail;

                    float futureRailPosition = tmpRailposition + _currentSpeed * JumpDuration / _rightRail.Length;

                    var splineSample = _rightRail.GetSampleAtDistance(futureRailPosition * _rightRail.Length);
                    Vector3 target = splineSample.location;

                    int jumpIndex = manager.GetNearPositionIndex(target);
                    _rightRailPosition = manager.GetNearRailPosition(jumpIndex);
                    right = manager.GetNearPosition(jumpIndex);
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
    /// レールポジションから座標取得
    /// </summary>
    #region レールポジションから座標取得
    public Vector3 PosRail(float _pos, Spline _rail)
    {
        var splineSample = CurrentRail.GetSampleAtDistance(_pos * _rail.Length);
        return splineSample.location;
    }
    #endregion
}

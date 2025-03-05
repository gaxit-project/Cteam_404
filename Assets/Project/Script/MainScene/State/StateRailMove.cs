using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using SplineMesh;


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
            //owner.animator.SetBool("isRide", true);
            _currentSpeed = owner.Speed;
        }

        public override void OnUpdate(Player owner)
        {

            owner.MoveAlongRail();
            owner.UpdateReferencePositions();

            #region プレイヤーレール操作

            _currentSpeed = owner.Speed;

            Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            float angle = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;
            if (input.magnitude > 0.1f) // 適当な閾値
            {
                if (angle >= 45 && angle < 135) // 上
                {
                    if (owner._leftPosition)
                    {
                        /*
                        if(owner._playerEmpty._leftRailPosition + owner._playerEmpty.MaxPos > owner._leftRailPosition)
                        {
                            owner._leftRailPosition = owner._playerEmpty._leftRailPosition + owner._playerEmpty.MaxPos;
                            owner.left = owner.PosRail(owner._playerEmpty._leftRailPosition + owner._playerEmpty.MaxPos, owner._leftRail);
                        }
                        */
                        owner.ChangeState(new StateJump(owner._leftRail, owner._leftRailPosition, owner.left));
                    }
                }
                else if (angle >= -135 && angle < -45) // 下
                {
                    if (owner._rightPosition)
                    {
                        /*
                        if (owner._playerEmpty._rightRailPosition + owner._playerEmpty.MaxPos > owner._rightRailPosition)
                        {
                            owner._rightRailPosition = owner._playerEmpty._rightRailPosition + owner._playerEmpty.MaxPos;
                            owner.right = owner.PosRail(owner._playerEmpty._rightRailPosition + owner._playerEmpty.MaxPos, owner._rightRail);
                        }
                        */
                        owner.ChangeState(new StateJump(owner._rightRail, owner._rightRailPosition, owner.right));
                    }
                }
                else if (angle >= -45 && angle < 45) // 右
                {
                    _currentSpeed = owner.MaxSpeed;
                }
                else // 左
                {
                    _currentSpeed = owner.MinSpeed;
                }
            }


            // 攻撃
            if (owner.isAttacking)
            {
                owner.isAttacking= !owner.isAttacking;
                owner.ChangeState(stateAttack);
            }

            if (owner.canULT)
            {
                // 長押しの進捗を取得
                progress = owner._holdAction.GetTimeoutCompletionPercentage();
                // 進捗が1以上になったときの処理
                if (progress >= 1 && !gaugeActivated)
                {                    
                    owner._holdAction.Disable();  // Actionを一旦無効化
                    owner._holdAction.Enable();   // すぐに有効化して次の入力に備える
                    progress = 0.0f;

                    Debug.Log("ULT発動");
                    owner.ChangeState(stateUltAttack);
                }
            }
            #endregion

            Debug.Log("現在の速度:" + _currentSpeed);

            if(owner._railPosition >= owner._playerEmpty._railPosition + owner._playerEmpty.MaxPos)
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

                Vector3 referenceObject = manager.GetNearPosition(closestIndex);
                float distance = Vector3.Distance(transform.position, referenceObject);

                if (distance > _snapDistance) continue; // スナップ距離外の場合スキップ

                Vector3 toObject = referenceObject - transform.position;
                float dot = Vector3.Dot(transform.right, toObject.normalized);

                if (dot < -0.5f && !_leftPosition) // 左側
                {
                    _leftPosition = true;
                    _leftRail = manager.TargetRail;
                    _leftRailPosition = manager.GetJumpRailPosition(closestIndex);
                    left = manager.GetJumpPosition(closestIndex);
                }
                else if (dot > 0.5f && !_rightPosition) // 右側
                {
                    _rightPosition = true;
                    _rightRail = manager.TargetRail;
                    _rightRailPosition = manager.GetJumpRailPosition(closestIndex);
                    right = manager.GetJumpPosition(closestIndex);
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

using UnityEngine;

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
            owner.animator.SetBool("isRide", true);
            _currentSpeed = owner.Speed;
        }

        public override void OnUpdate(Player owner)
        {
            owner.MoveAlongRail();
            owner.UpdateReferencePositions();

            #region プレイヤーレール操作

            // レール移動
            if (Input.GetKeyDown(KeyCode.W) && owner._leftPosition)
            {
                owner.ChangeState(new StateJump(owner._leftRail, owner._leftRailPosition, owner.left));
            }
            else if (Input.GetKeyDown(KeyCode.S) && owner._rightPosition)
            {
                owner.ChangeState(new StateJump(owner._rightRail, owner._rightRailPosition, owner.right));
            }

            // レール加減速
            _currentSpeed = owner.Speed;
            if (Input.GetKey(KeyCode.A))
            {
                _currentSpeed = owner.MinSpeed;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                _currentSpeed = owner.MaxSpeed;
            }

            // 攻撃
            if (Input.GetKeyDown("h"))
            {
                Debug.Log("攻撃");
                owner.ChangeState(stateAttack);
            }
            #endregion

            Debug.Log("現在の速度:" + _currentSpeed);

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
                transform.forward = splineSample.tangent;
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












                /*for (int i = 0; i < manager.ReferenceObjects.Length; i++)
                {
                    Vector3 referenceObject = manager.GetNearPosition(i);
                    float distance = Vector3.Distance(transform.position, referenceObject);

                    if (distance > _snapDistance) continue; // スナップ距離外の場合スキップ

                    Vector3 toObject = referenceObject - transform.position;
                    float dot = Vector3.Dot(transform.right, toObject.normalized);

                    if (dot < -0.5f && !_leftPosition) // 左側
                    {
                        _leftPosition = true;
                        _leftRail = manager.TargetRail;
                        _leftRailPosition = manager.GetJumpRailPosition(i);
                        left = manager.GetJumpPosition(i);
                    }
                    else if (dot > 0.5f && !_rightPosition) // 右側
                    {
                        _rightPosition = true;
                        _rightRail = manager.TargetRail;
                        _rightRailPosition = manager.GetJumpRailPosition(i);
                        right = manager.GetJumpPosition(i);
                    }
                }*/
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("UpdateReferencePositionsで例外が発生しました: " + ex.Message);
        }
    }
    #endregion

}

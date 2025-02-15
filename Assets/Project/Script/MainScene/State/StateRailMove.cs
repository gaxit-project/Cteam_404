using UnityEngine;

public partial class Player
{
    /// <summary>
    /// ���[����̈ړ��X�e�[�g
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

            #region �v���C���[���[������

            // ���[���ړ�
            if (Input.GetKeyDown(KeyCode.W) && owner._leftPosition)
            {
                owner.ChangeState(new StateJump(owner._leftRail, owner._leftRailPosition, owner.left));
            }
            else if (Input.GetKeyDown(KeyCode.S) && owner._rightPosition)
            {
                owner.ChangeState(new StateJump(owner._rightRail, owner._rightRailPosition, owner.right));
            }

            // ���[��������
            _currentSpeed = owner.Speed;
            if (Input.GetKey(KeyCode.A))
            {
                _currentSpeed = owner.MinSpeed;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                _currentSpeed = owner.MaxSpeed;
            }

            // �U��
            if (Input.GetKeyDown("h"))
            {
                Debug.Log("�U��");
                owner.ChangeState(stateAttack);
            }
            #endregion

            Debug.Log("���݂̑��x:" + _currentSpeed);

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
                    owner._railPosition = 0f; // ���[�v����
                }
            }
        }
    }

    /// <summary>
    /// ���[����̌��݂̈ʒu�ƌ������X�V
    /// </summary>
    #region ���[����̌��݂̈ʒu�ƌ������X�V
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
                Debug.LogError("MoveAlongRail�ŗ�O���������܂���: " + ex.Message);
            }
        }

    }
    #endregion

    /// <summary>
    /// ���̃��[���̎Q�Ɨp�I�u�W�F�N�g�𒲍����č��E�̃��[���|�W�V�������X�V
    /// </summary>
    #region ���̃��[���̎Q�Ɨp�I�u�W�F�N�g�𒲍����č��E�̃��[���|�W�V�������X�V
    void UpdateReferencePositions()
    {
        try
        {
            RailManager[] railManagers = FindObjectsOfType<RailManager>();

            // ��Ԃ����Z�b�g
            _leftPosition = false;
            _rightPosition = false;
            _leftRail = null;
            _rightRail = null;

            foreach (var manager in railManagers)
            {
                // ���݂̃��[���̓X�L�b�v
                if (manager.TargetRail == CurrentRail) continue;

                int closestIndex = manager.GetNearPositionIndex(transform.position);
                if (closestIndex == -1) continue; // �L���ȎQ�Ƃ��Ȃ��ꍇ�X�L�b�v

                Vector3 referenceObject = manager.GetNearPosition(closestIndex);
                float distance = Vector3.Distance(transform.position, referenceObject);

                if (distance > _snapDistance) continue; // �X�i�b�v�����O�̏ꍇ�X�L�b�v

                Vector3 toObject = referenceObject - transform.position;
                float dot = Vector3.Dot(transform.right, toObject.normalized);

                if (dot < -0.5f && !_leftPosition) // ����
                {
                    _leftPosition = true;
                    _leftRail = manager.TargetRail;
                    _leftRailPosition = manager.GetJumpRailPosition(closestIndex);
                    left = manager.GetJumpPosition(closestIndex);
                }
                else if (dot > 0.5f && !_rightPosition) // �E��
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

                    if (distance > _snapDistance) continue; // �X�i�b�v�����O�̏ꍇ�X�L�b�v

                    Vector3 toObject = referenceObject - transform.position;
                    float dot = Vector3.Dot(transform.right, toObject.normalized);

                    if (dot < -0.5f && !_leftPosition) // ����
                    {
                        _leftPosition = true;
                        _leftRail = manager.TargetRail;
                        _leftRailPosition = manager.GetJumpRailPosition(i);
                        left = manager.GetJumpPosition(i);
                    }
                    else if (dot > 0.5f && !_rightPosition) // �E��
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
            Debug.LogError("UpdateReferencePositions�ŗ�O���������܂���: " + ex.Message);
        }
    }
    #endregion

}

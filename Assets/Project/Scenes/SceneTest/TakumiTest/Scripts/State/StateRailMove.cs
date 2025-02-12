using UnityEngine;

public partial class Player
{
    /// <summary>
    /// ���[����̈ړ��X�e�[�g
    /// </summary>
    public class StateRailMove : PlayerStateBase
    {
        public override void OnEnter(Player owner, PlayerStateBase prevState)
        {
            owner.animator.SetBool("isRide", true);
        }

        public override void OnUpdate(Player owner)
        {
            owner._railPosition += owner.Speed * Time.deltaTime / owner.CurrentRail.Length;
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

            owner.MoveAlongRail();
            owner.UpdateReferencePositions();

            if (Input.GetKeyDown(KeyCode.A) && owner._leftPosition)
            {
                owner.ChangeState(new StateJump(owner._leftRail, owner._leftRailPosition, owner.left));
            }
            else if (Input.GetKeyDown(KeyCode.D) && owner._rightPosition)
            {
                owner.ChangeState(new StateJump(owner._rightRail, owner._rightRailPosition, owner.right));
            }

            // Enter�L�[�ōU��
            if (Input.GetKeyDown("h"))
            {
                Debug.Log("�U��");
                owner.ChangeState(stateAttack);
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

                for (int i = 0; i < manager.ReferenceObjects.Length; i++)
                {
                    Vector3 referenceObject = manager.GetNearPosition(i);
                    float distance = Vector3.Distance(transform.position, referenceObject);

                    if (distance > _snapDistance) continue; // �X�i�b�v�����O�̏ꍇ�X�L�b�v

                    Vector3 toObject = referenceObject - transform.position;
                    float dot = Vector3.Dot(Vector3.right, toObject.normalized);

                    if (dot > 0.5f && !_rightPosition) // ����
                    {
                        _leftPosition = true;
                        _leftRail = manager.TargetRail;
                        _leftRailPosition = manager.GetNearRailPosition(i);
                        left = manager.GetNearPosition(i);
                    }
                    else if (dot < -0.5f && !_leftPosition) // �E��
                    {
                        _rightPosition = true;
                        _rightRail = manager.TargetRail;
                        _rightRailPosition = manager.GetNearRailPosition(i);
                        right = manager.GetNearPosition(i);
                    }
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("UpdateReferencePositions�ŗ�O���������܂���: " + ex.Message);
        }
    }
    #endregion

}

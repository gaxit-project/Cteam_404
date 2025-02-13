using UnityEngine;
using SplineMesh;

public partial class Player
{

    /// <summary>
    /// �U���X�e�[�g
    /// </summary>
    public class StateAttack : PlayerStateBase
    {
        float time = 0;
        public override void OnEnter(Player owner, PlayerStateBase prevState)
        {
            time = 0;
            owner.animator.SetTrigger("isAttack");
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
            // �U�����̓��ʂȓ��삪����ꍇ�͂����ɒǉ�

            time += Time.deltaTime;
            if(time >= 1f)
            {
                owner.ChangeState(stateRailMove);
            }
        }


    }
}

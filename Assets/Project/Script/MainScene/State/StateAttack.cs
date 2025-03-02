using UnityEngine;
using SplineMesh;

public partial class Player
{

    /// <summary>
    /// 攻撃ステート
    /// </summary>
    public class StateAttack : PlayerStateBase
    {
        float time = 0;
        public override void OnEnter(Player owner, PlayerStateBase prevState)
        {
            time = 0;
            owner.arms.SetActive(true);
            //owner.animator.SetTrigger("isAttack");
            //ここでSE
            AudioManager.GetInstance().PlaySound(5);
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
                    owner._railPosition = 0f; // ループ処理
                }
            }

            owner.MoveAlongRail();
            // 攻撃中の特別な動作がある場合はここに追加

            time += Time.deltaTime;
            if(time >= 1f)
            {
                owner.arms.SetActive(false);
                owner.ChangeState(stateRailMove);
            }
        }


    }
}

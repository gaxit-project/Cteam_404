using UnityEngine;



public partial class Player
{
    /// <summary>
    /// ULT攻撃ステート
    /// </summary>
    public class StateULTAttack : PlayerStateBase
    {
        private float time = 0;
        private BossHealth health;
        public override void OnEnter(Player owner, PlayerStateBase prevState)
        {
            health = owner._boss.GetComponent<BossHealth>();
            owner._mobCounter = 0; //撃破カウントをリセット
            owner.canULT = false;
            owner.isULT = true;
            owner.particle.Play();  //ビームエフェクトを再生
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

            if(time >= owner._ultTime - (owner._ultTime / 4))
            {
                owner.particle.Stop();
            }

            if (time >= owner._ultTime)
            {
                health.TakeDamage(owner._damegeULT);
                owner.isULT = false;
                owner.ChangeState(stateRailMove);
            }
        }
    }
}


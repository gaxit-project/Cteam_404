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
            owner.UltStay.Stop();
            owner.particle.Play();  //ビームエフェクトを再生
            time = 0f;
            AudioManager.GetInstance().PlaySound(7);
        }

        public override void OnUpdate(Player owner)
        {
            #region レール移動
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
            #endregion
            // 攻撃中の特別な動作がある場合はここに追加

            owner.UltGauge -= (1f / owner._ultTime) * Time.deltaTime;
            time += Time.deltaTime;

            if(time >= owner._ultTime - (owner._ultTime / 4))
            {
                owner.particle.Stop();
            }

            if (owner.UltGauge <= 0f)
            {
                owner.main.startSpeed = 0f;
                owner.emission.rateOverTime = 0f;
                health.TakeDamage(owner._damegeULT);
                owner.isULT = false;
                owner.arms.SetActive(false);
                owner.ChangeState(stateRailMove);
            }
        }
    }
}


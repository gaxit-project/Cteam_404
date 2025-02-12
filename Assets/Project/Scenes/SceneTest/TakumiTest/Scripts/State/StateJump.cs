using UnityEngine;
using SplineMesh;

public partial class Player
{
    /// <summary>
    /// ジャンプステート
    /// </summary>
    public class StateJump : PlayerStateBase
    {
        private Spline targetRail;
        private float targetPosition;
        private Vector3 target;

        public StateJump(Spline targetRail, float targetPosition, Vector3 target)
        {
            this.targetRail = targetRail;
            this.targetPosition = targetPosition;
            this.target = target;
        }

        public override void OnEnter(Player owner, PlayerStateBase prevState)
        {
            owner._isJumping = true;
            owner.StartCoroutine(owner.JumpCoroutine(owner, owner.transform.position, target, owner.transform.forward, () =>
            {
                owner.CurrentRail = targetRail;
                owner._railPosition = targetPosition;
                owner._isJumping = false;
                owner.ChangeState(new StateRailMove());
            }));
        }
    }

    /// <summary>
    /// ジャンプアニメーション
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="onComplete"></param>
    /// <returns></returns>
    #region ジャンプアニメーション
    private System.Collections.IEnumerator JumpCoroutine(Player owner, Vector3 start, Vector3 end, Vector3 direction, System.Action onComplete)
    {
        float elapsed = 0f;

        while (elapsed < owner.JumpDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / JumpDuration;

            // ジャンプの軌跡 (放物線)
            float height = Mathf.Sin(t * Mathf.PI) * JumpHeight;
            transform.position = Vector3.Lerp(start, end, t) + Vector3.up * height;

            // 進行方向をスムーズに更新
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, t);

            yield return null;
        }

        onComplete?.Invoke();
    }

    #endregion

}
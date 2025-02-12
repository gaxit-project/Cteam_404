using UnityEngine;
using SplineMesh;

public partial class Player
{
    /// <summary>
    /// �W�����v�X�e�[�g
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
    /// �W�����v�A�j���[�V����
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="onComplete"></param>
    /// <returns></returns>
    #region �W�����v�A�j���[�V����
    private System.Collections.IEnumerator JumpCoroutine(Player owner, Vector3 start, Vector3 end, Vector3 direction, System.Action onComplete)
    {
        float elapsed = 0f;

        while (elapsed < owner.JumpDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / JumpDuration;

            // �W�����v�̋O�� (������)
            float height = Mathf.Sin(t * Mathf.PI) * JumpHeight;
            transform.position = Vector3.Lerp(start, end, t) + Vector3.up * height;

            // �i�s�������X���[�Y�ɍX�V
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, t);

            yield return null;
        }

        onComplete?.Invoke();
    }

    #endregion

}
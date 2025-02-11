using UnityEngine;

public class BossLaserAttack : MonoBehaviour
{
    public Transform centerObject; // 円の中心
    public Transform player; // プレイヤー
    public float lookBackTime = 0.5f; // どれくらい前の位置を取得するか
    public float attackDuration = 3f; // 攻撃が続く時間（秒）

    private Vector3 predictedPlayerPosition; // 予測したプレイヤーの少し前の座標
    private bool isAttacking = false;
    private float attackTimer = 0f;
    private float previousAngle = 0f; // プレイヤーの前フレームの角度

    void Update()
    {
        if (isAttacking)
        {
            // 攻撃中は座標を固定する
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0)
            {
                isAttacking = false; // 攻撃終了
            }
        }
        else
        {
            // プレイヤーの円周上の情報を取得
            Vector3 toPlayer = player.position - centerObject.position;
            float radius = toPlayer.magnitude;

            // 現在の角度を求める (XZ平面)
            float currentAngle = Mathf.Atan2(toPlayer.z, toPlayer.x);

            // プレイヤーの角速度を計算（前フレームとの差分）
            float angularVelocity = (currentAngle - previousAngle) / Time.deltaTime;

            // 過去の角度を計算
            float pastAngle = currentAngle - angularVelocity * lookBackTime;

            // 円周上の「少し前の位置」を計算
            predictedPlayerPosition = centerObject.position + new Vector3(Mathf.Cos(pastAngle), 0, Mathf.Sin(pastAngle)) * radius;

            // 角度を更新
            previousAngle = currentAngle;
        }

        // デバッグ用の表示
        Debug.DrawLine(centerObject.position, player.position, Color.green); // 現在の位置
        Debug.DrawLine(centerObject.position, predictedPlayerPosition, Color.red); // 少し前の位置
    }

    // 外部から呼び出し可能な攻撃開始メソッド
    public void StartAttack()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            attackTimer = attackDuration;
        }
    }

    // 攻撃対象の座標を取得
    public Vector3 GetTargetPosition()
    {
        return predictedPlayerPosition;
    }
}

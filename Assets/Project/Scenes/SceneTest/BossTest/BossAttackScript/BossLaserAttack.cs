using UnityEngine;

public class BossLaserAttack : MonoBehaviour
{
    [Header("円設定")]
    [InspectorName("円の中心")]
    public Transform centerObject;
    [InspectorName("プレイヤーオブジェクト")]
    public Transform player;

    [Header("レーザー設定")]
    [InspectorName("レーザーパーティクル")]
    public ParticleSystem laserParticle;
    [InspectorName("レーザー発射角度(度数法)")]
    public float forwardOffsetAngle = 10f;
    [InspectorName("攻撃時間")]
    public float attackDuration = 3f;

    private Vector3 targetPosition; // 固定されたターゲット位置
    private bool isAttacking = false;
    private float attackTimer = 0f;

    void Update()
    {
        if (isAttacking)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0)
            {
                isAttacking = false;
                laserParticle.Stop();
            }
            Debug.DrawLine(centerObject.position, targetPosition, Color.red); // ターゲット位置
        }
    }

    public void StartAttack()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            attackTimer = attackDuration;

            // 攻撃開始時にターゲット位置を固定
            targetPosition = PredictPlayerFuturePosition();

            // パーティクルシステムの位置と向きを設定
            laserParticle.transform.position = centerObject.position;
            laserParticle.transform.LookAt(targetPosition);

            // パーティクルシステムを再生
            laserParticle.Play();
        }
    }

    /// <summary>
    /// プレイヤーの未来位置を予測するメソッド
    /// </summary>
    /// <returns></returns>
    private Vector3 PredictPlayerFuturePosition()
    {
        // プレイヤーの現在の半径（円周の大きさ）を取得
        float dynamicRadius = Vector3.Distance(centerObject.position, player.position);

        // プレイヤーの現在の位置から円の中心へのベクトルを求める
        Vector3 radiusVector = (player.position - centerObject.position).normalized;

        // プレイヤーの現在の角度を求める
        float currentAngle = Mathf.Atan2(radiusVector.z, radiusVector.x) * Mathf.Rad2Deg;

        // 指定した角度分、進行方向に前方の座標を求める
        float targetAngle = currentAngle + forwardOffsetAngle;
        float radians = targetAngle * Mathf.Deg2Rad;

        // 動的な半径に基づいて円周上の新しい位置を計算
        Vector3 predictedPosition = new Vector3(
            centerObject.position.x + Mathf.Cos(radians) * dynamicRadius,
            player.position.y, // 高さはプレイヤーと同じ
            centerObject.position.z + Mathf.Sin(radians) * dynamicRadius
        );

        return predictedPosition;
    }
}

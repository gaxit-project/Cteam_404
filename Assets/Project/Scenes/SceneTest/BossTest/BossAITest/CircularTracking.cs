using UnityEngine;

public class PlayerTracking : MonoBehaviour
{
    public Transform centerObject; // 円の中心
    public Transform player; // プレイヤー
    public float forwardOffsetAngle = 10f; // どれくらい前方の座標を取得するか（角度）

    void Update()
    {
        // 毎フレーム、プレイヤーの現在の半径（円周の大きさ）を取得
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

        // デバッグ用の表示
        Debug.DrawLine(centerObject.position, player.position, Color.green); // 現在の位置
        Debug.DrawLine(player.position, predictedPosition, Color.red); // 前方の予測位置

        // ここで predictedPosition をボスのビームのターゲット位置に設定
    }
}

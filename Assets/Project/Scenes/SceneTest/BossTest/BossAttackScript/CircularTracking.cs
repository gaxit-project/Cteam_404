using UnityEngine;

public class PlayerTracking : MonoBehaviour
{
    public Transform centerObject; // 円の中心
    public Transform player; // プレイヤー
    public float lookBackTime = 0.5f; // どれくらい前の位置を取得するか

    private Vector3 previousPlayerPosition;
    private Vector3 predictedPlayerPosition;

    void Update()
    {
        // 半径を計算（プレイヤーが円周上にいると仮定）
        float radius = Vector3.Distance(centerObject.position, player.position);

        // プレイヤーの速度を計算
        Vector3 playerVelocity = (player.position - previousPlayerPosition) / Time.deltaTime;

        // 予測位置（少し前の位置を取得）
        Vector3 pastPosition = player.position - playerVelocity * lookBackTime;

        // 中心から半径分の距離を保つように補正
        Vector3 direction = (pastPosition - centerObject.position).normalized;
        predictedPlayerPosition = centerObject.position + direction * radius;

        // デバッグ用の表示
        Debug.DrawLine(centerObject.position, player.position, Color.green); // 現在の位置
        Debug.DrawLine(centerObject.position, predictedPlayerPosition, Color.red); // 少し前の位置

        // 次のフレーム用に現在の位置を保存
        previousPlayerPosition = player.position;
    }
}

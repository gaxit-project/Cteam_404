using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("ターゲット設定")]
    public Transform player; // プレイヤーのTransform
    public Transform boss;   // ボスのTransform

    [Header("カメラ移動設定")]
    public float baseDistance = 5f; // プレイヤーからの基本距離
    public float minDistance = 3f;  // 最小距離
    public float maxDistance = 12f; // 最大距離
    public float heightOffset = 1.5f; // プレイヤーの頭上位置
    public float lateralOffset = 1.0f; // カメラの横位置補正

    [Header("ばねカメラ設定")]
    public float springStrength = 10f; // ばねの強さ
    public float damping = 5f;         // 減衰（ダンピング）

    [Header("FOV設定")]
    public float minFOV = 40f;  // 最小FOV
    public float maxFOV = 80f;  // 最大FOV
    public float fovPadding = 2f; // 余白
    public float fovSmoothing = 5f; // FOVのスムージング

    private Vector3 velocity; // カメラ移動の速度
    private Camera cam;       // カメラコンポーネント

    private void Start()
    {
        cam = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        if (player == null || boss == null) return;

        // 1. プレイヤーとボスの中間点を取得
        Vector3 midpoint = (player.position + boss.position) / 2;

        // 2. カメラの視点位置を計算（プレイヤーの後方に配置）
        Vector3 playerToBoss = (boss.position - player.position).normalized; // プレイヤー→ボスの方向
        Vector3 targetCameraPosition = player.position
                                      - playerToBoss * baseDistance // プレイヤーの後ろに配置
                                      + Vector3.up * heightOffset   // 少し上にオフセット
                                      + Vector3.right * lateralOffset; // 横方向の補正

        // 3. プレイヤーとボスの距離に応じてカメラの距離を調整
        float dynamicDistance = Mathf.Clamp(Vector3.Distance(player.position, boss.position), minDistance, maxDistance);
        targetCameraPosition = player.position - playerToBoss * dynamicDistance + Vector3.up * heightOffset;

        // 4. ばねカメラの移動計算
        Vector3 springForce = (targetCameraPosition - transform.position) * springStrength;
        Vector3 dampingForce = -velocity * damping;
        velocity += (springForce + dampingForce) * Time.deltaTime;
        transform.position += velocity * Time.deltaTime;

        // 5. ボスを見つめるようにカメラを回転
        transform.LookAt(boss.position);

        // 6. FOVを調整してプレイヤーとボスを収める
        AdjustFOV();
    }

    private void AdjustFOV()
    {
        float targetDistance = Vector3.Distance(player.position, boss.position);
        float targetFOV = Mathf.Clamp(targetDistance + fovPadding, minFOV, maxFOV);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * fovSmoothing);
    }
}

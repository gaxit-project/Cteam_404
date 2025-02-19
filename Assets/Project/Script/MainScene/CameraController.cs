using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("ターゲット設定")]
    public Transform player; // プレイヤーのTransform
    public Transform boss;   // ボスのTransform

    [Header("カメラ移動設定")]
    public float baseDistance = 10f; // プレイヤーからの基本距離
    public float minDistance = 5f;  // 最小距離
    public float maxDistance = 23f; // 最大距離
    public float heightOffset = 20f; // プレイヤーの頭上位置
    public float lateralOffset = 1f; // カメラの横位置補正
    public float cameraMinDistance = 50f; // カメラが最も近づく距離
    public float cameraMaxDistance = 70f; // カメラが最も遠ざかる距離

    [Header("ばねカメラ設定")]
    public float springStrength = 10f; // ばねの強さ
    public float damping = 5f;         // 減衰（ダンピング）

    [Header("FOV設定")]
    public float minFOV = 50f;  // 最小FOV
    public float maxFOV = 90f;  // 最大FOV
    public float fovPadding = 5f; // 余白
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

        // 1. プレイヤーとボスの距離を取得
        float playerToBossDistance = Vector3.Distance(player.position, boss.position);

        // 2. カメラの距離を動的に変更（ボスが近ければ遠ざけ、ボスが遠ければ近づける）
        float dynamicDistance = Mathf.Lerp(cameraMaxDistance, cameraMinDistance, playerToBossDistance / maxDistance);
        dynamicDistance = Mathf.Clamp(dynamicDistance, cameraMinDistance, cameraMaxDistance);

        // 3. カメラのターゲット位置を計算
        Vector3 playerToBoss = (boss.position - player.position).normalized; // プレイヤー→ボスの方向
        Vector3 targetCameraPosition = player.position - playerToBoss * dynamicDistance + Vector3.up * heightOffset + Vector3.right * lateralOffset;

        // 4. ばねカメラの移動計算
        Vector3 springForce = (targetCameraPosition - transform.position) * springStrength;
        Vector3 dampingForce = -velocity * damping;
        velocity += (springForce + dampingForce) * Time.deltaTime;
        transform.position += velocity * Time.deltaTime;

        // 5. ボスを見つめるようにカメラを回転
        transform.LookAt(boss.position);

        // 6. FOVを調整して3レーンを常に視界に入れる
        AdjustFOV(playerToBossDistance);
    }

    private void AdjustFOV(float playerToBossDistance)
    {
        // 3レーンが視界に収まるようにFOVを調整
        float targetFOV = Mathf.Lerp(maxFOV, minFOV, playerToBossDistance / maxDistance);
        targetFOV = Mathf.Clamp(targetFOV + fovPadding, minFOV, maxFOV);

        // なめらかに補間
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * fovSmoothing);
    }
}

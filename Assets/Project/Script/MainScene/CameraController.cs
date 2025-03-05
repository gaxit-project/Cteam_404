using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("ターゲット設定")]
    public Transform player; // プレイヤーのTransform（カメラが追従する対象）
    public Transform playerEmp; // プレイヤーのTransform（カメラが追従する対象）
    public Transform boss;   // ボスのTransform（視点の基準となるもう一つの対象）

    [Header("カメラ移動設定")]
    public float baseDistance = 10f; // プレイヤーからの基本的なカメラの距離
    public float minDistance = 5f;  // カメラがプレイヤーに最も近づく距離
    public float maxDistance = 23f; // カメラがプレイヤーから最も遠ざかる距離
    public float heightOffset = 20f; // カメラの高さ（プレイヤーの頭上の位置）
    public float lateralOffset = 1f; // カメラの横方向のオフセット（左右の位置調整）
    public float cameraMinDistance = 50f; // カメラが最も近づく制限距離
    public float cameraMaxDistance = 70f; // カメラが最も遠ざかる制限距離

    [Header("ばねカメラ設定")]
    public float springStrength = 10f; // カメラが目標位置へ追従する際のばねの強さ（大きいほど早く追従）
    public float damping = 5f;         // カメラの減衰（高いほど揺れが少なくなる）

    [Header("FOV設定")]
    public float minFOV = 50f;  // 最小FOV（視野角が狭い状態）
    public float maxFOV = 90f;  // 最大FOV（視野角が広い状態）
    public float fovPadding = 5f; // FOVの余白（視野に余裕を持たせるため）
    public float fovSmoothing = 5f; // FOVの変更速度（値が高いほどなめらかに変化）

    private Vector3 velocity; // カメラ移動時の速度（ばねカメラ用）
    private Camera cam;       // カメラコンポーネント

    private void Start()
    {
        cam = GetComponent<Camera>(); // カメラのコンポーネントを取得
    }

    private void LateUpdate()
    {
        if (player == null || boss == null) return; // プレイヤーかボスが存在しない場合は処理しない

        // 1. プレイヤーとボスの距離を取得
        float playerToBossDistance = Vector3.Distance(playerEmp.position, boss.position);

        // 2. カメラの距離を動的に変更
        // ボスが近づくとカメラは遠ざかり、ボスが遠ざかるとカメラは近づく
        float dynamicDistance = Mathf.Lerp(cameraMaxDistance, cameraMinDistance, playerToBossDistance / maxDistance);
        dynamicDistance = Mathf.Clamp(dynamicDistance, cameraMinDistance, cameraMaxDistance); // 設定範囲内に制限

        // 3. カメラの目標位置を計算
        // ボスの方向にカメラを配置し、プレイヤーを視点の基準にする
        Vector3 playerToBoss = (boss.position - player.position).normalized; // プレイヤー→ボスの方向ベクトル
        Vector3 targetCameraPosition = player.position
                                      - playerToBoss * dynamicDistance // プレイヤーの後方に配置
                                      + Vector3.up * heightOffset // 指定した高さにオフセット
                                      + Vector3.right * lateralOffset; // 横方向の位置補正

        // 4. ばねカメラの移動計算（スムーズな移動）
        Vector3 springForce = (targetCameraPosition - transform.position) * springStrength; // 目標位置に向かう力
        Vector3 dampingForce = -velocity * damping; // 振動を抑えるための減衰力
        velocity += (springForce + dampingForce) * Time.deltaTime; // 速度を更新
        transform.position += velocity * Time.deltaTime; // 新しい位置に適用

        // 5. ボスを見つめるようにカメラを回転
        transform.LookAt(boss.position);

        // 6. FOVを調整して3レーンが常に視界に収まるようにする
        AdjustFOV(playerToBossDistance);
    }

    private void AdjustFOV(float playerToBossDistance)
    {
        // 3レーンが視界に収まるようにFOVを調整
        // ボスが遠くなると視野を狭め、近くなると視野を広げる
        float targetFOV = Mathf.Lerp(maxFOV, minFOV, playerToBossDistance / maxDistance);
        targetFOV = Mathf.Clamp(targetFOV + fovPadding, minFOV, maxFOV); // FOVの範囲を制限

        // なめらかに補間してFOVを変更（急激な変化を防ぐ）
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * fovSmoothing);
    }
}

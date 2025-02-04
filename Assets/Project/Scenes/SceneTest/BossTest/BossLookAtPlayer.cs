using UnityEngine;

public class BossLookAtPlayer : MonoBehaviour
{
    [Header("プレイヤータグ")]
    [SerializeField] private string playerTag = "Player";

    [Header("回転速度")]
    [SerializeField] private float rotationSpeed = 5f;

    private Transform playerTransform;

    private void Start()
    {
        GameObject playerObject = GameObject.FindWithTag(playerTag);
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
    }

    private void Update()
    {
        if (playerTransform == null) return;

        // プレイヤーとの方向を計算 (Y軸のみ)
        Vector3 directionToPlayer = playerTransform.position - transform.position;
        directionToPlayer.y = 0f; // Y軸を無視して水平面上のみを考慮

        // プレイヤーが真上または真下にいる場合の処理を回避
        if (directionToPlayer.sqrMagnitude < 0.01f) return;

        // 現在の前方とプレイヤー方向を補間して回転
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer.normalized);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
using UnityEngine;
using SplineMesh;

public class Missile : MonoBehaviour
{
    [SerializeField] private float speed = 10f; // 追従速度
    [SerializeField] private float dropSpeed = 5f; // 落下速度
    [SerializeField] private float explosionRadius = 2f; // 爆発範囲（視覚用）
    private Vector3 targetPosition; // 頭上ターゲット
    private Vector3 impactPosition; // 着陸点
    private bool isDropping = false; // 落下中かどうか
    private Player player; // プレイヤーの参照
    private RailManager railManager; // RailManagerの参照

    void Start()
    {
        player = FindObjectOfType<Player>();
        railManager = FindObjectOfType<RailManager>();
        if (railManager == null)
        {
            Debug.LogError("RailManagerが見つかりません。シーンにRailManagerを配置してください。");
        }
    }

    public void SetTarget(Vector3 target, Vector3 impact, float missileSpeed, float dropSpd, float radius)
    {
        targetPosition = target;
        impactPosition = impact;
        speed = missileSpeed;
        dropSpeed = dropSpd;
        explosionRadius = radius;
    }

    void Update()
    {
        if (!isDropping)
        {
            // 頭上ターゲットに向かって追従
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            // 頭上ターゲットに到達したら落下開始
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                isDropping = true;
            }
        }
        else
        {
            // 着陸点に向かって落下（レール進行に追従）
            if (railManager != null && player != null)
            {
                Vector3 railDirection = GetRailDirection();
                impactPosition += railDirection * player.Speed * Time.deltaTime; // プレイヤーの速度で追従
            }
            transform.position = Vector3.MoveTowards(transform.position, impactPosition, dropSpeed * Time.deltaTime);

            // 着陸点に到達したら爆発（視覚エフェクトのみ）
            if (Vector3.Distance(transform.position, impactPosition) < 0.1f)
            {
                Explode();
            }
        }
    }

    private Vector3 GetRailDirection()
    {
        if (railManager != null && railManager.TargetRail != null)
        {
            int nearIndex = railManager.GetNearPositionIndex(player.transform.position);
            float railPosition = railManager.GetNearRailPosition(nearIndex);
            if (railPosition < 0) railPosition = 0f;

            float nextPosition = railPosition + 0.01f;
            if (nextPosition > 1f) nextPosition -= 1f;
            Vector3 nextPoint = railManager.TargetRail.GetSample(nextPosition).location; // GetSampleを使用
            Vector3 currentPoint = railManager.TargetRail.GetSample(railPosition).location; // GetSampleを使用
            return (nextPoint - currentPoint).normalized;
        }
        return Vector3.forward; // デフォルト方向
    }

    private void Explode()
    {
        // 視覚的な爆発エフェクト（必要に応じてParticleSystemを追加）
        Debug.Log("ミサイルが着陸点で爆発");

        Destroy(gameObject); // ミサイルを削除
    }

    void OnDrawGizmosSelected()
    {
        // 爆発範囲をGizmosで表示（デバッグ用）
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(impactPosition, explosionRadius);
    }
}
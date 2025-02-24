using UnityEngine;

public class LineWallSetup : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer; // LineRendererの参照
    [SerializeField] private GameObject playerHitPrefab; // PlayerHitプレハブ
    [SerializeField] private float wallThickness = 0.1f; // 壁の厚さ（X方向）
    [SerializeField] private float wallHeight = 5f; // 壁の高さ（Y方向）
    [SerializeField] private float wallOffset = 0.5f; // 線からのオフセット（Z方向）

    private GameObject[] wallSegments; // 配置した壁セグメントの配列

    void Start()
    {
        if (lineRenderer == null || playerHitPrefab == null)
        {
            Debug.LogError("LineRendererまたはPlayerHitプレハブがアサインされていません。");
            return;
        }

        SetupWalls();
    }

    void LateUpdate()
    {
        UpdateWalls(); // 線の位置が変更された場合に壁を更新
    }

    void SetupWalls()
    {
        // 既存の壁セグメントを削除
        if (wallSegments != null)
        {
            foreach (GameObject segment in wallSegments)
            {
                if (segment != null) Destroy(segment);
            }
        }

        // LineRendererのポイント数を取得
        int pointCount = lineRenderer.positionCount;

        if (pointCount < 2) return;

        // 各セグメントごとに壁を配置（左右に2つ、合計2 * (pointCount - 1)個）
        wallSegments = new GameObject[2 * (pointCount - 1)];
        for (int i = 0; i < pointCount - 1; i++)
        {
            Vector3 point1 = lineRenderer.GetPosition(i);
            Vector3 point2 = lineRenderer.GetPosition(i + 1);

            Vector3 segmentDirection = (point2 - point1).normalized;
            float segmentLength = Vector3.Distance(point1, point2);
            Vector3 wallDirection = Vector3.Cross(segmentDirection, Vector3.up).normalized;

            for (int side = 0; side < 2; side++) // 左（0）、右（1）
            {
                Vector3 wallPosition = ((point1 + point2) * 0.5f) + (wallDirection * (side == 0 ? -wallOffset : wallOffset));
                Quaternion wallRotation = Quaternion.LookRotation(segmentDirection, Vector3.up);

                GameObject wall = Instantiate(playerHitPrefab, wallPosition, wallRotation, transform);
                wall.transform.localScale = new Vector3(wallThickness, wallHeight, segmentLength);

                BoxCollider collider = wall.GetComponent<BoxCollider>();
                if (collider != null)
                {
                    collider.size = new Vector3(wallThickness, wallHeight, segmentLength);
                    collider.center = new Vector3(0, 0, segmentLength * 0.5f);
                }

                wallSegments[i * 2 + side] = wall; // 配列に保存（左右の壁を分ける）
            }
        }
    }

    void UpdateWalls()
    {
        if (lineRenderer == null || wallSegments == null)
        {
            return;
        }

        int pointCount = lineRenderer.positionCount;
        if (pointCount < 2 || wallSegments.Length != 2 * (pointCount - 1))
        {
            SetupWalls();
            return;
        }

        // 既存の壁の位置を更新
        for (int i = 0; i < pointCount - 1; i++)
        {
            Vector3 point1 = lineRenderer.GetPosition(i);
            Vector3 point2 = lineRenderer.GetPosition(i + 1);

            Vector3 segmentDirection = (point2 - point1).normalized;
            float segmentLength = Vector3.Distance(point1, point2);
            Vector3 wallDirection = Vector3.Cross(segmentDirection, Vector3.up).normalized;

            for (int side = 0; side < 2; side++)
            {
                int index = i * 2 + side; // インデックス計算
                if (index < wallSegments.Length && wallSegments[index] != null)
                {
                    GameObject wall = wallSegments[index];
                    Vector3 wallPosition = ((point1 + point2) * 0.5f) + (wallDirection * (side == 0 ? -wallOffset : wallOffset));
                    Quaternion wallRotation = Quaternion.LookRotation(segmentDirection, Vector3.up);

                    wall.transform.position = wallPosition;
                    wall.transform.rotation = wallRotation;
                    wall.transform.localScale = new Vector3(wallThickness, wallHeight, segmentLength);

                    BoxCollider collider = wall.GetComponent<BoxCollider>();
                    if (collider != null)
                    {
                        collider.size = new Vector3(wallThickness, wallHeight, segmentLength);
                        collider.center = new Vector3(0, 0, segmentLength * 0.5f);
                    }
                }
                else
                {
                    Debug.LogWarning($"壁セグメントのインデックス {index} が範囲外または null です。");
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        if (lineRenderer != null && playerHitPrefab != null)
        {
            int pointCount = lineRenderer.positionCount;
            for (int i = 0; i < pointCount - 1; i++)
            {
                Vector3 point1 = lineRenderer.GetPosition(i);
                Vector3 point2 = lineRenderer.GetPosition(i + 1);
                Vector3 segmentDirection = (point2 - point1).normalized;
                float segmentLength = Vector3.Distance(point1, point2);
                Vector3 wallDirection = Vector3.Cross(segmentDirection, Vector3.up).normalized;

                Gizmos.color = Color.green;
                for (int side = -1; side <= 1; side += 2)
                {
                    Vector3 wallPosition = ((point1 + point2) * 0.5f) + (wallDirection * side * wallOffset);
                    Vector3 wallSize = new Vector3(wallThickness, wallHeight, segmentLength);
                    Gizmos.DrawWireCube(wallPosition, wallSize);
                }
            }
        }
    }
}
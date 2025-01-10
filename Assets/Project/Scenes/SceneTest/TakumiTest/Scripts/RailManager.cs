using UnityEngine;
using SplineMesh;

public class RailManager : MonoBehaviour
{
    [Header("対象のレール")]
    public Spline TargetRail;             // 管理対象のレール

    [Header("参照用オブジェクト")]
    [SerializeField] private GameObject _referencePrefab; // レール上に配置する参照用オブジェクトのプレハブ
    [SerializeField] private float _spacing = 1f;         // 参照用オブジェクトの間隔（メートル単位）

    public GameObject[] ReferenceObjects;  // 配置した参照用オブジェクトの配列
    public float[] RailPositions;          // 各オブジェクトに対応するスプライン上の位置（0〜1）

    void Start()
    {
        GenerateReferenceObjects();
    }

    /// <summary>
    /// レール上に参照用オブジェクトを生成し、対応するスプライン位置を計算
    /// </summary>
    private void GenerateReferenceObjects()
    {
        // レールの長さを取得
        float railLength = TargetRail.Length;

        // 必要なオブジェクトの数を計算
        int objectCount = Mathf.CeilToInt(railLength / _spacing);

        // 配列を初期化
        ReferenceObjects = new GameObject[objectCount];
        RailPositions = new float[objectCount];

        // 参照用オブジェクトを生成
        for (int i = 0; i < objectCount; i++)
        {
            // レール上の距離を計算
            float distance = i * _spacing;

            // スプライン上の位置情報を取得
            var sample = TargetRail.GetSampleAtDistance(distance);

            // オブジェクトを生成して配置
            GameObject referenceObject = Instantiate(_referencePrefab, sample.location, Quaternion.identity, transform);
            referenceObject.transform.forward = sample.tangent; // レールの進行方向に向きを設定

            // 配列に保存
            ReferenceObjects[i] = referenceObject;
            RailPositions[i] = distance / railLength; // スプライン全体での位置を0〜1で保存
        }
    }

    public Spline GetRail()
    {
        return TargetRail;
    }

    /// <summary>
    /// 最も近い参照用オブジェクトを取得
    /// </summary>
    /// <param name="position">基準となる3D位置</param>
    /// <returns>最も近いオブジェクトの配列インデックス</returns>
    public int GetNearPositionIndex(Vector3 position)
    {
        int closestIndex = -1;
        float closestDistance = float.MaxValue;

        for (int i = 0; i < ReferenceObjects.Length; i++)
        {
            float distance = Vector3.Distance(position, ReferenceObjects[i].transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestIndex = i;
            }
        }

        return closestIndex;
    }
    /// <summary>
    /// indexに対応した参照用オブジェクトのpositionを取得
    /// </summary>
    /// <param name="index">参照用オブジェクトのindex</param>
    /// <returns>参照用オブジェクトのposition(Vector3型)</returns>
    public Vector3 GetNearPosition(int index)
    {
        //Debug.Assert(index < 0 || ReferenceObjects.Length < index ,"要素数外を参照しようとしています");
        return ReferenceObjects[index].transform.position;
    }

    /// <summary>
    /// indexに対応した参照用オブジェクトのレールポジション取得
    /// </summary>
    /// <param name="index">オブジェクトの配列インデックス</param>
    /// <returns>スプライン上の位置（0〜1）</returns>
    public float GetNearRailPosition(int index)
    {
        if (index >= 0 && index < RailPositions.Length)
        {
            return RailPositions[index];
        }

        Debug.LogWarning("指定されたインデックスが範囲外です。");
        return -1f; // 範囲外の場合のエラー値
    }
}

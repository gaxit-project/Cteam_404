using System.Collections.Generic;
using UnityEngine;
using SplineMesh;

public class RailManager : MonoBehaviour
{
    [Header("参照用オブジェクトの間隔 (メートル単位)")]
    [SerializeField] private float _interval = 1f;

    [Header("参照用オブジェクトのプレハブ")]
    [SerializeField] private GameObject _referencePointPrefab;

    private Spline _spline;
    private List<Transform> _referencePoints = new List<Transform>();

    void Start()
    {
        _spline = GetComponent<Spline>();
        if (_spline == null)
        {
            Debug.LogError("RailManagerはSplineコンポーネントと一緒に使用する必要があります。");
            return;
        }

        GenerateReferencePoints();
    }

    /// <summary>
    /// レール上に一定間隔で参照用オブジェクトを配置
    /// </summary>
    private void GenerateReferencePoints()
    {
        float totalLength = _spline.Length;

        for (float distance = 0f; distance <= totalLength; distance += _interval)
        {
            // レール上の位置を計算
            var sample = _spline.GetSampleAtDistance(distance);
            Vector3 position = sample.location;

            // 参照用オブジェクトを生成
            GameObject referencePoint = Instantiate(_referencePointPrefab, position, Quaternion.identity, transform);
            _referencePoints.Add(referencePoint.transform);
        }
    }

    /// <summary>
    /// 現在の位置に最も近い参照用オブジェクトを返す
    /// </summary>
    /// <param name="position">現在のワールド座標</param>
    /// <returns>最も近い参照用オブジェクトのTransform</returns>
    public Transform GetClosestReferencePoint(Vector3 position)
    {
        Transform closestPoint = null;
        float closestDistance = float.MaxValue;

        foreach (Transform point in _referencePoints)
        {
            float distance = Vector3.Distance(position, point.position);
            if (distance < closestDistance)
            {
                closestPoint = point;
                closestDistance = distance;
            }
        }

        return closestPoint;
    }
}

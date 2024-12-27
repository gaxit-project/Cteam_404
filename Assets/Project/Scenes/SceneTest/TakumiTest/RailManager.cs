using System.Collections.Generic;
using UnityEngine;
using SplineMesh;

public class RailManager : MonoBehaviour
{
    [Header("�Q�Ɨp�I�u�W�F�N�g�̊Ԋu (���[�g���P��)")]
    [SerializeField] private float _interval = 1f;

    [Header("�Q�Ɨp�I�u�W�F�N�g�̃v���n�u")]
    [SerializeField] private GameObject _referencePointPrefab;

    private Spline _spline;
    private List<Transform> _referencePoints = new List<Transform>();

    void Start()
    {
        _spline = GetComponent<Spline>();
        if (_spline == null)
        {
            Debug.LogError("RailManager��Spline�R���|�[�l���g�ƈꏏ�Ɏg�p����K�v������܂��B");
            return;
        }

        GenerateReferencePoints();
    }

    /// <summary>
    /// ���[����Ɉ��Ԋu�ŎQ�Ɨp�I�u�W�F�N�g��z�u
    /// </summary>
    private void GenerateReferencePoints()
    {
        float totalLength = _spline.Length;

        for (float distance = 0f; distance <= totalLength; distance += _interval)
        {
            // ���[����̈ʒu���v�Z
            var sample = _spline.GetSampleAtDistance(distance);
            Vector3 position = sample.location;

            // �Q�Ɨp�I�u�W�F�N�g�𐶐�
            GameObject referencePoint = Instantiate(_referencePointPrefab, position, Quaternion.identity, transform);
            _referencePoints.Add(referencePoint.transform);
        }
    }

    /// <summary>
    /// ���݂̈ʒu�ɍł��߂��Q�Ɨp�I�u�W�F�N�g��Ԃ�
    /// </summary>
    /// <param name="position">���݂̃��[���h���W</param>
    /// <returns>�ł��߂��Q�Ɨp�I�u�W�F�N�g��Transform</returns>
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

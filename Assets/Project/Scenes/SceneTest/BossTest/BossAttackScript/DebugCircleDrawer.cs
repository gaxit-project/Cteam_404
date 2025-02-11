using UnityEngine;
using UnityEditor;

[ExecuteInEditMode] // シーンビューで実行
public class DebugCircleDrawer : MonoBehaviour
{
    public Transform centerObject;
    public Transform player;

    void OnDrawGizmos()
    {
        if (centerObject == null || player == null) return;

        float radius = Vector3.Distance(centerObject.position, player.position);
        Handles.color = Color.yellow;
        Handles.DrawWireDisc(centerObject.position, Vector3.up, radius);
    }
}

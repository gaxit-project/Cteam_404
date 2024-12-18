
using UnityEngine;

public class CameraMoveContoller : MonoBehaviour
{
    // キャラクターオブジェクト
    public GameObject playerObj;
    // カメラとの距離
    private Vector3 offset;

    void Start()
    {
        offset = transform.position - playerObj.transform.position;
    }

    void LateUpdate()
    {
        transform.position = playerObj.transform.position + offset;
    }
}


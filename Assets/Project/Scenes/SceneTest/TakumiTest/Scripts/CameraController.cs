using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("プレイヤー")]
    [SerializeField] GameObject _player;

    private Vector3 offset;


    private void Start()
    {
        Debug.Assert( _player != null , "追従用のプレイヤーをアタッチしろ!!!");
        offset = transform.position - _player.transform.position;
    }

    private void LateUpdate()
    {
        transform.position = _player.transform.position + offset;
    }
}

using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("�v���C���[")]
    [SerializeField] GameObject _player;

    private Vector3 offset;


    private void Start()
    {
        Debug.Assert( _player != null , "�Ǐ]�p�̃v���C���[���A�^�b�`����!!!");
        offset = transform.position - _player.transform.position;
    }

    private void LateUpdate()
    {
        transform.position = _player.transform.position + offset;
    }
}

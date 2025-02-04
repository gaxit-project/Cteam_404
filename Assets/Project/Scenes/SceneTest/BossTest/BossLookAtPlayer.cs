using UnityEngine;

public class BossLookAtPlayer : MonoBehaviour
{
    [Header("�v���C���[�^�O")]
    [SerializeField] private string playerTag = "Player";

    [Header("��]���x")]
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

        // �v���C���[�Ƃ̕������v�Z (Y���̂�)
        Vector3 directionToPlayer = playerTransform.position - transform.position;
        directionToPlayer.y = 0f; // Y���𖳎����Đ����ʏ�݂̂��l��

        // �v���C���[���^��܂��͐^���ɂ���ꍇ�̏��������
        if (directionToPlayer.sqrMagnitude < 0.01f) return;

        // ���݂̑O���ƃv���C���[�������Ԃ��ĉ�]
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer.normalized);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
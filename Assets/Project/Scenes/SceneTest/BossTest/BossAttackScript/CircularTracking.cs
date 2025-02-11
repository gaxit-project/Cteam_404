using UnityEngine;

public class PlayerTracking : MonoBehaviour
{
    public Transform centerObject; // �~�̒��S
    public Transform player; // �v���C���[
    public float lookBackTime = 0.5f; // �ǂꂭ�炢�O�̈ʒu���擾���邩

    private Vector3 previousPlayerPosition;
    private Vector3 predictedPlayerPosition;

    void Update()
    {
        // ���a���v�Z�i�v���C���[���~����ɂ���Ɖ���j
        float radius = Vector3.Distance(centerObject.position, player.position);

        // �v���C���[�̑��x���v�Z
        Vector3 playerVelocity = (player.position - previousPlayerPosition) / Time.deltaTime;

        // �\���ʒu�i�����O�̈ʒu���擾�j
        Vector3 pastPosition = player.position - playerVelocity * lookBackTime;

        // ���S���甼�a���̋�����ۂ悤�ɕ␳
        Vector3 direction = (pastPosition - centerObject.position).normalized;
        predictedPlayerPosition = centerObject.position + direction * radius;

        // �f�o�b�O�p�̕\��
        Debug.DrawLine(centerObject.position, player.position, Color.green); // ���݂̈ʒu
        Debug.DrawLine(centerObject.position, predictedPlayerPosition, Color.red); // �����O�̈ʒu

        // ���̃t���[���p�Ɍ��݂̈ʒu��ۑ�
        previousPlayerPosition = player.position;
    }
}

using UnityEngine;

public class PlayerTracking : MonoBehaviour
{
    public Transform centerObject; // �~�̒��S
    public Transform player; // �v���C���[
    public float forwardOffsetAngle = 10f; // �ǂꂭ�炢�O���̍��W���擾���邩�i�p�x�j

    void Update()
    {
        // ���t���[���A�v���C���[�̌��݂̔��a�i�~���̑傫���j���擾
        float dynamicRadius = Vector3.Distance(centerObject.position, player.position);

        // �v���C���[�̌��݂̈ʒu����~�̒��S�ւ̃x�N�g�������߂�
        Vector3 radiusVector = (player.position - centerObject.position).normalized;

        // �v���C���[�̌��݂̊p�x�����߂�
        float currentAngle = Mathf.Atan2(radiusVector.z, radiusVector.x) * Mathf.Rad2Deg;

        // �w�肵���p�x���A�i�s�����ɑO���̍��W�����߂�
        float targetAngle = currentAngle + forwardOffsetAngle;
        float radians = targetAngle * Mathf.Deg2Rad;

        // ���I�Ȕ��a�Ɋ�Â��ĉ~����̐V�����ʒu���v�Z
        Vector3 predictedPosition = new Vector3(
            centerObject.position.x + Mathf.Cos(radians) * dynamicRadius,
            player.position.y, // �����̓v���C���[�Ɠ���
            centerObject.position.z + Mathf.Sin(radians) * dynamicRadius
        );

        // �f�o�b�O�p�̕\��
        Debug.DrawLine(centerObject.position, player.position, Color.green); // ���݂̈ʒu
        Debug.DrawLine(player.position, predictedPosition, Color.red); // �O���̗\���ʒu

        // ������ predictedPosition ���{�X�̃r�[���̃^�[�Q�b�g�ʒu�ɐݒ�
    }
}

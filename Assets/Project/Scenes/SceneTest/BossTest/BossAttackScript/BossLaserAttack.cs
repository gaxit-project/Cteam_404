using UnityEngine;

public class BossLaserAttack : MonoBehaviour
{
    public Transform centerObject; // �~�̒��S
    public Transform player; // �v���C���[
    public float lookBackTime = 0.5f; // �ǂꂭ�炢�O�̈ʒu���擾���邩
    public float attackDuration = 3f; // �U�����������ԁi�b�j

    private Vector3 predictedPlayerPosition; // �\�������v���C���[�̏����O�̍��W
    private bool isAttacking = false;
    private float attackTimer = 0f;
    private float previousAngle = 0f; // �v���C���[�̑O�t���[���̊p�x

    void Update()
    {
        if (isAttacking)
        {
            // �U�����͍��W���Œ肷��
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0)
            {
                isAttacking = false; // �U���I��
            }
        }
        else
        {
            // �v���C���[�̉~����̏����擾
            Vector3 toPlayer = player.position - centerObject.position;
            float radius = toPlayer.magnitude;

            // ���݂̊p�x�����߂� (XZ����)
            float currentAngle = Mathf.Atan2(toPlayer.z, toPlayer.x);

            // �v���C���[�̊p���x���v�Z�i�O�t���[���Ƃ̍����j
            float angularVelocity = (currentAngle - previousAngle) / Time.deltaTime;

            // �ߋ��̊p�x���v�Z
            float pastAngle = currentAngle - angularVelocity * lookBackTime;

            // �~����́u�����O�̈ʒu�v���v�Z
            predictedPlayerPosition = centerObject.position + new Vector3(Mathf.Cos(pastAngle), 0, Mathf.Sin(pastAngle)) * radius;

            // �p�x���X�V
            previousAngle = currentAngle;
        }

        // �f�o�b�O�p�̕\��
        Debug.DrawLine(centerObject.position, player.position, Color.green); // ���݂̈ʒu
        Debug.DrawLine(centerObject.position, predictedPlayerPosition, Color.red); // �����O�̈ʒu
    }

    // �O������Ăяo���\�ȍU���J�n���\�b�h
    public void StartAttack()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            attackTimer = attackDuration;
        }
    }

    // �U���Ώۂ̍��W���擾
    public Vector3 GetTargetPosition()
    {
        return predictedPlayerPosition;
    }
}

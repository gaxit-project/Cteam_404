using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private bool isDamaged = false;        // �_���[�W���󂯂Ă��邩
    private bool isInvincible = false;     // ���G���Ԃł��邩
    private bool isDebug = false;          // �f�o�b�O���[�h���L����
    private float damageTimer = 0f;        // �_���[�W���󂯂Ă���o�ߎ���
    private float invincibilityTimer = 0f; // ���G��Ԃ̌o�ߎ���
    private int damageCount = 0;           // �_���[�W���󂯂���

    [SerializeField]
    private float damageCooldownTime = 3f; // �_���[�W�̉񕜎���
    [SerializeField]
    private float invincibilityTime = 2f;  // ���G����

    void Update()
    {
        HandleDamageRecovery();
        HandleInvincibility();

        // �f�o�b�O�p�FD�L�[�Ńf�o�b�O���[�h�̐؂�ւ�
        if (Input.GetKeyDown(KeyCode.D))
        {
            ToggleDebugMode();
        }
    }

    // �_���[�W���󂯂Ă���̉񕜎��Ԃ��`�F�b�N���AisDamaged���X�V
    private void HandleDamageRecovery()
    {
        if (isDamaged)
        {
            damageTimer += Time.deltaTime;
            if (damageTimer >= damageCooldownTime)
            {
                isDamaged = false;
                damageTimer = 0f;
            }
        }
    }

    // ���G���Ԃ��`�F�b�N���AisInvincible���X�V
    private void HandleInvincibility()
    {
        if (isInvincible)
        {
            invincibilityTimer += Time.deltaTime;
            if (invincibilityTimer >= invincibilityTime)
            {
                isInvincible = false;
                invincibilityTimer = 0f;
            }
        }
    }

    // �_���[�W���󂯂��ۂ̏���
    public void TakeDamage()
    {
        // ���G�̏ꍇ�̓_���[�W�𖳌���
        if (isInvincible)
        {
            return;
        }

        // �_���[�W���󂯂Ă���Œ��ɍēx�_���[�W���󂯂���Q�[���I�[�o�[
        if (isDamaged)
        {
            if (!isDebug)
            {
                GameOver();
            }
            return;
        }

        // ����_���[�W����
        isDamaged = true;
        isInvincible = true;
        damageTimer = 0f; // �_���[�W�^�C�}�[�����Z�b�g
        invincibilityTimer = 0f; // ���G�^�C�}�[�����Z�b�g
        damageCount++;
    }

    // �Q�[���I�[�o�[���̏���
    public void GameOver()
    {
        // �Q�[���I�[�o�[�ɂȂ������ɋN����C�x���g�Ȃǂ�}������
    }

    // �f�o�b�O���[�h�̐؂�ւ�
    private void ToggleDebugMode()
    {
        isDebug = !isDebug;
    }
}

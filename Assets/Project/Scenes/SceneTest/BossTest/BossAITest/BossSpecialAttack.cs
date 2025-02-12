using UnityEngine;

public class BossSpecialAttack : MonoBehaviour
{
    [SerializeField] private ParticleSystem specialEffect; // �K�E�Z�̃p�[�e�B�N���G�t�F�N�g

    private void Update()
    {
        // K�L�[�������ꂽ��K�E�Z����
        if (Input.GetKeyDown(KeyCode.K))
        {
            ExecuteAttack();
        }
    }

    public void ExecuteAttack()
    {
        if (specialEffect != null)
        {
            specialEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            specialEffect.Play(); // �p�[�e�B�N�����Đ�
            Debug.Log("�K�E�Z�����I");
            Invoke(nameof(StopSpecialEffect), 3f);
        }
        else
        {
            Debug.LogWarning("�K�E�Z�G�t�F�N�g���A�^�b�`����Ă��܂���I");
        }
    }

    private void StopSpecialEffect()
    {
        if(specialEffect != null)
        {
            specialEffect.Stop();
            Debug.Log("�K�E�Z�I��");
        }
    }
}

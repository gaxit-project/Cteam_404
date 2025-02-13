using System.Collections.Generic;
using UnityEngine;

public class BossStateAI : MonoBehaviour
{
    enum State
    {
        doNothing,
        charging,
        specialAttack,
        attack
    }

    [SerializeField] private List<MonoBehaviour> attackScripts; // �U���X�N���v�g
    [SerializeField] private BossSpecialAttack specialAttackScript; // �K�E�Z�X�N���v�g
    [SerializeField] private float firstPhaseAttackInterval = 5f; // ���`�Ԃ̍U���Ԋu
    [SerializeField] private float secondPhaseAttackInterval = 2.5f; // ���`�Ԃ̍U���Ԋu
    [SerializeField] private float chargeTime = 3f; // �K�E�Z�`���[�W����
    [SerializeField] private bool isSecondPhase = false; // ���`�ԂɈڍs���邩

    private float attackInterval;
    private float attackTimer = 0f;
    private float chargeTimer = 0f;
    private State currentState = State.doNothing;
    private bool stateEnter = true;

    private void Start()
    {
        // ������Ԃ̍U���Ԋu��ݒ�
        attackInterval = isSecondPhase ? secondPhaseAttackInterval : firstPhaseAttackInterval;
    }

    private void Update()
    {

        attackTimer += Time.deltaTime;

        // ������2�L�[�ő��`�Ԃֈڍs�i�e�X�g�p�j
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            EnterSecondPhase();
        }

        // K�L�[�ŕK�E�Z�𔭓��i�e�X�g�p�j
        if (Input.GetKeyDown(KeyCode.K))
        {
            ChangeState(State.charging);
        }

        switch (currentState)
        {
            case State.doNothing:
                if (stateEnter)
                {
                    stateEnter = false;
                    Debug.Log("Boss�͔��΂�ł���");
                }

                if (attackTimer >= attackInterval)
                {
                    ChangeState(State.attack);
                    return;
                }
                break;

            case State.charging:
                if (stateEnter)
                {
                    stateEnter = false;
                    chargeTimer = 0f;
                    Debug.Log("�K�E�Z�`���[�W��");
                }

                chargeTimer += Time.deltaTime;
                if(chargeTimer >= chargeTime)
                {
                    ChangeState(State.specialAttack);
                }
                break;

            case State.specialAttack:
                if (stateEnter)
                {
                    stateEnter = false;
                    ExecuteSpecialAttack();
                }
                break;

            case State.attack:
                if (stateEnter)
                {
                    stateEnter = false;
                    ExecuteRandomAttack();
                }

                ChangeState(State.doNothing);
                break;
        }
    }

    private void ChangeState(State newState)
    {
        currentState = newState;
        stateEnter = true;
    }

    private void ExecuteRandomAttack()
    {
        if (attackScripts.Count == 0) return;

        int index = Random.Range(0, attackScripts.Count);
        MonoBehaviour attackScript = attackScripts[index];

        var method = attackScript.GetType().GetMethod("ExecuteAttack");
        if (method != null)
        {
            method.Invoke(attackScript, null);
            attackTimer = 0f; // �^�C�}�[�����Z�b�g
        }
        else
        {
            Debug.LogWarning($"{attackScript.name} �� ExecuteAttack ���\�b�h������܂���I");
        }
    }

    private void ExecuteSpecialAttack()
    {
        if (specialAttackScript != null)
        {
            specialAttackScript.ExecuteAttack();
            Debug.Log("�K�E�Z�J�n");
        }
        else
        {
            Debug.LogWarning($"{specialAttackScript.name} �� ExecuteAttack ���\�b�h������܂���I");
        }
    }
    
    public void SpecialAttackFinished()
    {
        ChangeState(State.doNothing);
    }

    /// <summary>
    /// ���`�Ԃֈڍs
    /// </summary>
    public void EnterSecondPhase()
    {
        if (!isSecondPhase)
        {
            isSecondPhase = true;
            attackInterval = secondPhaseAttackInterval; // �U���X�s�[�h�ύX
            Debug.Log("Boss�͑��`�Ԃɐi�������I");
        }
    }
}

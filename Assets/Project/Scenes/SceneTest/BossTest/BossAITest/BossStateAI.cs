using System.Collections.Generic;
using UnityEngine;

public class BossStateAI : MonoBehaviour
{
    enum State
    {
        doNothing,
        attack
    }

    [SerializeField] private List<MonoBehaviour> attackScripts; // 攻撃スクリプト
    [SerializeField] private MonoBehaviour specialAttackScript; // 必殺技スクリプト
    [SerializeField] private float firstPhaseAttackInterval = 5f; // 第一形態の攻撃間隔
    [SerializeField] private float secondPhaseAttackInterval = 2.5f; // 第二形態の攻撃間隔
    [SerializeField] private bool isSecondPhase = false; // 第二形態に移行するか

    private float attackInterval;
    private float attackTimer = 0f;
    private State currentState = State.doNothing;
    private bool stateEnter = true;

    private void Start()
    {
        // 初期状態の攻撃間隔を設定
        attackInterval = isSecondPhase ? secondPhaseAttackInterval : firstPhaseAttackInterval;
    }

    private void Update()
    {
        attackTimer += Time.deltaTime;

        // 数字の2キーで第二形態へ移行（テスト用）
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            EnterSecondPhase();
        }

        // Kキーで必殺技を発動
        if (Input.GetKeyDown(KeyCode.K))
        {
            ExecuteSpecialAttack();
        }

        switch (currentState)
        {
            case State.doNothing:
                if (stateEnter)
                {
                    stateEnter = false;
                    Debug.Log("Bossは微笑んでいる");
                }

                if (attackTimer >= attackInterval)
                {
                    ChangeState(State.attack);
                    return;
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
            attackTimer = 0f; // タイマーをリセット
        }
        else
        {
            Debug.LogWarning($"{attackScript.name} に ExecuteAttack メソッドがありません！");
        }
    }

    private void ExecuteSpecialAttack()
    {
        if (specialAttackScript == null) return;

        var method = specialAttackScript.GetType().GetMethod("ExecuteAttack");
        if (method != null)
        {
            method.Invoke(specialAttackScript, null);
        }
        else
        {
            Debug.LogWarning($"{specialAttackScript.name} に ExecuteAttack メソッドがありません！");
        }
    }

    /// <summary>
    /// 第二形態へ移行
    /// </summary>
    public void EnterSecondPhase()
    {
        if (!isSecondPhase)
        {
            isSecondPhase = true;
            attackInterval = secondPhaseAttackInterval; // 攻撃スピード変更
            Debug.Log("Bossは第二形態に進化した！");
        }
    }
}

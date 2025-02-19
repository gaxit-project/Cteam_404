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

    [Header("通常攻撃")]
    [InspectorName("通常攻撃スクリプト")]
    [SerializeField] private List<MonoBehaviour> attackScripts;

    [Header("必殺技")]
    [InspectorName("必殺技スクリプト")]
    [SerializeField] private BossSpecialAttack specialAttackScript;
    [InspectorName("チャージSE")]
    [SerializeField] private AudioSource chargeSound;
    [InspectorName("チャージエフェクト")]
    [SerializeField] private ParticleSystem chargeEffect;
    [InspectorName("チャージエフェクト停止タイミング")]
    [SerializeField, Range(0f, 1f)] private float chargeEffectStopRatio = 0.8f;
    [InspectorName("必殺技チャージ時間")]
    [SerializeField] private float chargeTime = 3f;

    [Header("第一形態ステータス")]
    [InspectorName("攻撃頻度")]
    [SerializeField] private float firstPhaseAttackInterval = 5f;

    [Header("第二形態ステータス")]
    [SerializeField] private bool isSecondPhase = false;
    [InspectorName("攻撃頻度")]
    [SerializeField] private float secondPhaseAttackInterval = 2.5f; // 第二形態の攻撃間隔

    private float attackInterval;
    private float attackTimer = 0f;
    private float chargeTimer = 0f;
    private State currentState = State.doNothing;
    private bool stateEnter = true;
    private Animator animator;

    private void Start()
    {
        // 初期状態の攻撃間隔を設定
        attackInterval = isSecondPhase ? secondPhaseAttackInterval : firstPhaseAttackInterval;
        animator = GetComponent<Animator>();

        if(chargeEffect != null)
        {
            chargeEffect.Stop();
        }
    }

    private void Update()
    {

        attackTimer += Time.deltaTime;

        // 数字の2キーで第二形態へ移行（テスト用）
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            EnterSecondPhase();
        }

        // Kキーで必殺技を発動（テスト用）
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
                    Debug.Log("必殺技チャージ中");

                    if (chargeEffect != null)
                    {
                        chargeEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear); ;
                        chargeEffect.Play();
                    }

                    if(chargeSound != null)
                    {
                        chargeSound.Play();
                    }

                    if (animator != null)
                    {
                        animator.SetTrigger("Charge");
                    }
                }

                chargeTimer += Time.deltaTime;

                if(chargeTimer >= chargeTime * chargeEffectStopRatio)
                {
                    if(chargeEffect != null && chargeEffect.isPlaying)
                    {
                        chargeEffect.Stop();
                    }

                    if (chargeEffect != null && chargeEffect.isPlaying)
                    {
                        chargeSound.Stop();
                    }
                }

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

        if(currentState != State.charging)
        {
            StopChargeEffect();
            StopSound();
        }
    }

    private void StopChargeEffect()
    {
        if(chargeSound != null)
        {
            chargeEffect.Stop();
        }
    }

    private void StopSound()
    {
        if(chargeSound != null)
        {
            chargeSound.Stop();
        }
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
        if (specialAttackScript != null)
        {
            specialAttackScript.ExecuteAttack();
            Debug.Log("必殺技開始");
        }
        else
        {
            Debug.LogWarning($"{specialAttackScript.name} に ExecuteAttack メソッドがありません！");
        }

        if(chargeEffect != null)
        {
            chargeEffect.Stop();
        }
    }
    
    public void SpecialAttackFinished()
    {
        ChangeState(State.doNothing);
    }

    /// <summary>
    /// 第二形態へ移行
    /// </summary>
    public void EnterSecondPhase()
    {
        if (!isSecondPhase)
        {
            isSecondPhase = true;
            attackInterval = secondPhaseAttackInterval;
            Debug.Log("Bossは第二形態に進化した！");
        }
    }
}

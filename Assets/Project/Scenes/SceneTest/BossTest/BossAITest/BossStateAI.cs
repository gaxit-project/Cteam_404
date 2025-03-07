using System.Collections;
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
    [InspectorName("必殺技発動間隔")]
    [SerializeField] private float specialAttackInterval = 30f;

    [Header("第一形態ステータス")]
    [InspectorName("攻撃頻度")]
    [SerializeField] private float firstPhaseAttackInterval = 5f;

    [Header("第二形態ステータス")]
    [SerializeField] private bool isSecondPhase = false;
    [InspectorName("攻撃頻度")]
    [SerializeField] private float secondPhaseAttackInterval = 2.5f;

    [Header("第三形態ステータス")]
    [SerializeField] private bool isThirdPhase = false;
    [InspectorName("攻撃頻度")]
    [SerializeField] private float thirdPhaseAttackInterval = 1.5f;
    [Header("第三形態専用技")]
    [SerializeField] private List<MonoBehaviour> thirdPhaseAttackScripts;
    
    private float attackInterval;
    private float attackTimer = 0f;
    private float chargeTimer = 0f;
    private State currentState = State.doNothing;
    private bool stateEnter = true;
    private bool isSpecialAttackReady = true;
    private Animator animator;

    private int lastAttackIndex = -1;
    private int repeatCount = 0;
    private const int maxRepeat = 3;

    private void Start()
    {
        attackInterval = isThirdPhase ? thirdPhaseAttackInterval : isSecondPhase ? secondPhaseAttackInterval : firstPhaseAttackInterval;
        animator = GetComponent<Animator>();

        if (chargeEffect != null)
        {
            chargeEffect.Stop();
        }

        StartCoroutine(DelayedSpecialAttackStart());
    }

    private IEnumerator DelayedSpecialAttackStart()
    {
        yield return new WaitForSeconds(30f);
        StartCoroutine(AutomaticSpecialAttack());
    }

    private void Update()
    {
        attackTimer += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            EnterSecondPhase();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            EnterThirdPhase();
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

                    if (chargeEffect != null)
                    {
                        chargeEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear); ;
                        chargeEffect.Play();
                    }

                    if (chargeSound != null)
                    {
                        chargeSound.Play();
                    }

                    if (animator != null)
                    {
                        animator.SetTrigger("Charge");
                    }
                }

                chargeTimer += Time.deltaTime;

                if (chargeTimer >= chargeTime * chargeEffectStopRatio)
                {
                    if (chargeEffect != null && chargeEffect.isPlaying)
                    {
                        chargeEffect.Stop();
                    }

                    if (chargeEffect != null && chargeEffect.isPlaying)
                    {
                        chargeSound.Stop();
                    }
                }

                if (chargeTimer >= chargeTime)
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

        if (currentState != State.charging)
        {
            StopChargeEffect();
            StopSound();
        }
    }

    private void StopChargeEffect()
    {
        if (chargeSound != null)
        {
            chargeEffect.Stop();
        }
    }

    private void StopSound()
    {
        if (chargeSound != null)
        {
            chargeSound.Stop();
        }
    }

    private void ExecuteRandomAttack()
    {
        if (attackScripts.Count == 0) return;

        List<MonoBehaviour> availableAttacks = attackScripts;

        if (isThirdPhase && thirdPhaseAttackScripts.Count > 0)
        {
            availableAttacks = thirdPhaseAttackScripts;
        }

        int index;

        if (repeatCount >= maxRepeat)
        {
            List<int> validIndices = new List<int>();

            for (int i = 0; i < attackScripts.Count; i++)
            {
                if (i != lastAttackIndex)
                {
                    validIndices.Add(i);
                }
            }

            if (validIndices.Count > 0)
            {
                index = validIndices[Random.Range(0, validIndices.Count)];
            }
            else
            {
                index = Random.Range(0, attackScripts.Count);
            }

            repeatCount = 0;
        }
        else
        {
            index = Random.Range(0, attackScripts.Count);

            if (index == lastAttackIndex)
            {
                repeatCount++;
            }
            else
            {
                repeatCount = 1;
            }
        }

        lastAttackIndex = index;

        MonoBehaviour attackScript = attackScripts[index];

        var method = attackScript.GetType().GetMethod("ExecuteAttack");
        if (method != null)
        {
            method.Invoke(attackScript, null);
            attackTimer = 0f;
        }
    }

    private void ExecuteSpecialAttack()
    {
        if (specialAttackScript != null)
        {
            specialAttackScript.ExecuteAttack();
        }

        if (chargeEffect != null)
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

    public void EnterThirdPhase()
    {
        if (!isThirdPhase)
        {
            isThirdPhase = true;
            isSecondPhase = false;
            attackInterval = thirdPhaseAttackInterval;
            Debug.Log("Bossは第三形態に進化した！");
        }
    }

    /// <summary>
    /// 30秒ごとに必殺技を発動するコルーチン
    /// </summary>
    private IEnumerator AutomaticSpecialAttack()
    {
        while (true)
        {
            if (isSpecialAttackReady)
            {
                ChangeState(State.charging);
                isSpecialAttackReady = false;
                yield return new WaitForSeconds(specialAttackInterval);
                isSpecialAttackReady = true;
            }
            yield return null;
        }
    }

    public void BossFacePhase()
    {
        if (!isSecondPhase && !isThirdPhase)
        {
            BossFace.Instance.ChangeFace(0);
        }
        if (isSecondPhase && !isThirdPhase)
        {
            BossFace.Instance.ChangeFace(1);
        }
        if (!isSecondPhase && isThirdPhase)
        {
            BossFace.Instance.ChangeFace(2);
        }
    }
}

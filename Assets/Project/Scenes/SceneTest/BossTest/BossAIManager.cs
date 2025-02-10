using NUnit.Framework;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;

public class BossAIManager : MonoBehaviour
{
    [Header("�U�����X�g")]
    [SerializeField] private List<GameObject> attackPrefabs;

    [Header("�U���Ԋu")]
    [SerializeField] private float minAttackInterval = 2f;
    [SerializeField] private float maxAttackInterval = 5f;

    private bool isAttacking = false;

    void Start()
    {
        StartCoroutine(AttackRoutine());
    }

    /// <summary>
    /// �U���������_���I�������s
    /// </summary>
    private IEnumerator AttackRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minAttackInterval, maxAttackInterval));

            if (attackPrefabs.Count > 0)
            {
                ExecuteRandomAttack();
            }
        }
    }

    /// <summary>
    /// �����_���U�����s
    /// </summary>
    private void ExecuteRandomAttack()
    {
        if (attackPrefabs.Count == 0) return;

        GameObject attackObject = Instantiate(attackPrefabs[Random.Range(0, attackPrefabs.Count)]);
        IBossAttack attackScript = attackObject.GetComponent<IBossAttack>();

        if(attackScript != null)
        {
            attackScript.ExecuteAttack(gameObject);
        }
        else
        {
            Debug.LogWarning("IBossAttack��������܂���");
        }
    }

    /// <summary>
    /// �U���I���܂őҋ@
    /// </summary>
    private IEnumerator WaitForAttackToEnd(MonoBehaviour attackScript)
    {
        System.Reflection.MethodInfo isAttackingMethod = attackScript.GetType().GetMethod("IsAttacking");
        while (isAttackingMethod != null && (bool)isAttackingMethod.Invoke(attackScript, null))
        {
            yield return null;
        }
        isAttacking = false;
    }

    /// <summary>
    /// �U���p�x�ύX
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    public void SetAttackInterval(float min, float max)
    {
        minAttackInterval = min;
        maxAttackInterval = max;
    }

}

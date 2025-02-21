using UnityEngine;
using System.Collections;

public class BossSpecialAttack : MonoBehaviour
{
    [SerializeField] private ParticleSystem specialEffect;
    [SerializeField] private Collider attackCollider;
    [SerializeField] private AudioSource specialAttackSound;
    [SerializeField] private int damageAmount = 10;

    private BossStateAI bossAI;

    private void Start()
    {
        bossAI = FindObjectOfType<BossStateAI>();
    }

    public void ExecuteAttack()
    {
        if (specialEffect != null)
        {
            specialEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            specialEffect.Play();

            if(specialAttackSound != null)
            {
                specialAttackSound.Play();
            }

            if (attackCollider != null)
            {
                attackCollider.enabled = true;
            }

            Invoke(nameof(StopSpecialEffect), 3f);
        }
    }

    private void StopSpecialEffect()
    {
        if(specialEffect != null)
        {
            specialEffect.Stop();
        }

        if(attackCollider != null)
        {
            attackCollider.enabled = false;
        }

        if(specialAttackSound != null)
        {
            specialAttackSound.Stop();
        }

        bossAI.SpecialAttackFinished();
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage();
            }
        }
    }
}

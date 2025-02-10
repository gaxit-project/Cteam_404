using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("HP�ݒ�")]
    [SerializeField] protected int maxHealth = 100;
    protected int currentHealth;

    protected virtual void Start()
    {
        currentHealth = maxHealth;�@//HP������
    }

    /// <summary>
    /// �_���[�W���󂯂鏈��
    /// </summary>
    public virtual void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    /// <summary>
    /// �G���|���ꂽ�Ƃ��̏���
    /// </summary>
    protected virtual void Die()
    {
        Debug.Log(gameObject.name + " ��|�����I"); //�m�F�p
        Destroy(gameObject);
    }
}

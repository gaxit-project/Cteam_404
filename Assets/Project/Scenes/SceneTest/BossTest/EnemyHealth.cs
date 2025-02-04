using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("HP設定")]
    [SerializeField] protected int maxHealth = 100;
    protected int currentHealth;

    protected virtual void Start()
    {
        currentHealth = maxHealth;　//HP初期化
    }

    /// <summary>
    /// ダメージを受ける処理
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
    /// 敵が倒されたときの処理
    /// </summary>
    protected virtual void Die()
    {
        Debug.Log(gameObject.name + " を倒した！"); //確認用
        Destroy(gameObject);
    }
}

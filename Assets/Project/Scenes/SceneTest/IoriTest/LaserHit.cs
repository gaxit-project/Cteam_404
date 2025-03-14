using UnityEngine;

public class LaserHit : MonoBehaviour
{
    [Header("障害物の設定")]
    [SerializeField] private string _playerTag = "Player";

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(_playerTag))
        {
            DealDamage(collision.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(_playerTag))
        {
            DealDamage(other.gameObject);
        }
    }

    /// <summary>
    /// プレイヤーにダメージを与える処理
    /// </summary>
    private void DealDamage(GameObject target)
    {
        PlayerHealth playerHealth = target.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            SceneChangeManager.GetInstance().GameOver();
            
        }
    }
}

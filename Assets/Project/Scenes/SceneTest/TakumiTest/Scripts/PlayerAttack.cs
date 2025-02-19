using Unity.VisualScripting;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private PlayerBeam _playerBeam;
    private PlayerController _playerController;

    private void Start()
    {
        _playerBeam = GetComponentInParent<PlayerBeam>();

        if(_playerBeam == null)
        {
            Debug.Log("PlayerBeam スクリプトが見つかりません");
        }

        // PlayerController を取得
        _playerController = GetComponentInParent<PlayerController>();

        if (_playerController == null)
        {
            Debug.LogError("PlayerController が見つかりません！");
        }
    }

    public void TriggerAttack()
    {
        //PlayerControllerが存在する場合、攻撃終了処理を実行
        if (_playerController != null)
        {
            _playerController.EndAttack();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_playerController.IsAttacking() && collision.collider.CompareTag("Mob"))
        {
            Debug.Log("ヒット");

            if (_playerBeam != null)
            {
                _playerBeam.AddMobHit();
            }
                
            Destroy(collision.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter が呼ばれました: " + other.name);
        
        if (_playerController.IsAttacking() && other.CompareTag("Mob"))
        {
            Debug.Log("ヒット");

        　　if (_playerBeam != null)
            {
                _playerBeam.AddMobHit();
            }
            Destroy(other.gameObject);
        }
    }
}

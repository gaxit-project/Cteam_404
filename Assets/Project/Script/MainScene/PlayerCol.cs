using System.Collections;
using UnityEngine;

public class PlayerCol : MonoBehaviour
{
    public ArrayList checkList;
    public PlayerAttack _playerAttack;
    private PlayerHealth _playerHealth;


    void Start()
    {
        checkList = new ArrayList();
        _playerHealth = GameObject.Find("Player").GetComponent<PlayerHealth>();
    }


    void LateUpdate()
    {
        if (_playerAttack.isHit)
        {
            _playerAttack.MobHit();
            Destroy(_playerAttack.mob);
        }

        if(!_playerAttack.isHit && _playerHealth.isHit)
        {
            _playerHealth.TakeDamage();
        }


        if (checkList.Contains("Mob"))
        {

            

            return;
        }

        _playerAttack.isHit = false;
        _playerHealth.isHit = false;
    }
}

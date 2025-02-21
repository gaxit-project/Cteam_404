using Unity.VisualScripting;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private Player _playerScript;


    private void Start()
    {
        _playerScript = GameObject.Find("Player").GetComponent<Player>();
    }



    private void OnCollisionEnter(Collision collision)
    {
        if (/*_playerScript. && */collision.collider.CompareTag("Mob"))
        {
            Debug.Log("ヒット1");
            _playerScript.AddMobHit();
            Destroy(collision.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter が呼ばれました: " + other.name);
        
        if (/*_playerScript. && */other.CompareTag("Mob"))
        {
            Debug.Log("ヒット2");
            _playerScript.AddMobHit();
            Destroy(other.gameObject);
        }
    }
}

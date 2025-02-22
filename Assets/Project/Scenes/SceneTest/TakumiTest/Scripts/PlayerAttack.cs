using Unity.VisualScripting;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private Player _playerScript;
    private PlayerCol _playerCol;
    public bool isHit = false;
    public GameObject mob;


    private void Start()
    {
        _playerScript = GameObject.Find("Player").GetComponent<Player>();
        _playerCol = GameObject.Find("Player").GetComponent<PlayerCol>();
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
            mob = other.gameObject;
            isHit = true;
            //MobHit();
            //Destroy(other.gameObject);
        }
    }

    public void MobHit()
    {
        _playerScript.AddMobHit();
    }
}

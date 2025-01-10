using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("MOOB"))
        {
            Debug.Log("�j��");
            Destroy(collision.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MOOB"))
        {
            Debug.Log("�j��");
            Destroy(other.gameObject);
        }
    }
}

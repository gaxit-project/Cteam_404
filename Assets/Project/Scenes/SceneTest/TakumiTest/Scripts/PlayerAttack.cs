using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Mob"))
        {
            Debug.Log("”j‰ó");
            Destroy(collision.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Mob"))
        {
            Debug.Log("”j‰ó");
            Destroy(other.gameObject);
        }
    }
}

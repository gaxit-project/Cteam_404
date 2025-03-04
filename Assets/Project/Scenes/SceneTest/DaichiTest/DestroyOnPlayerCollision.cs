using UnityEngine;

public class DestroyOnPlayerCollision : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Waepon"))
        {
            AudioManager.GetInstance().PlaySound(9);
        }
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Waepon"))
        {
            AudioManager.GetInstance().PlaySound(9);
        }
    }
}

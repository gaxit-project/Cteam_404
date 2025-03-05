using UnityEngine;
using System.Collections;

public class Missile : MonoBehaviour
{
    [SerializeField] private GameObject warningAreaPrefab;
    [SerializeField] private float speed = 10f;
    private GameObject warningArea;
    private Vector3 targetPosition;  //ミサイルの目標座標
    private Vector3 landingPosition;  //着弾点（警告エリアの位置）
    private bool isTargetReached = false;  //目標に到達したかどうか

    public void SetTarget(Vector3 target, Vector3 landing, float missileSpeed, GameObject warning)
    {
        targetPosition = target;
        landingPosition = landing;
        speed = missileSpeed;
        warningArea = warning;
    }

    void Update()
    {
        if (!isTargetReached)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                isTargetReached = true;
                StartCoroutine(DropToLanding());
            }
        }
    }

    private IEnumerator DropToLanding()
    {
        while (Vector3.Distance(transform.position, landingPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, landingPosition, speed * Time.deltaTime);
            yield return null;
        }

        DestroyMissileAndWarning();
    }

    private void DestroyMissileAndWarning()
    {
        if (warningArea != null)
        {
            Destroy(warningArea);
            AudioManager.GetInstance().PlaySound(12);
        }
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == warningArea)
        {
            DestroyMissileAndWarning();
        }
    }
}

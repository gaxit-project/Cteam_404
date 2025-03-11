using UnityEngine;
using System.Collections;

public class Missile : MonoBehaviour
{
    [SerializeField] private GameObject warningAreaPrefab;
    [SerializeField] private float speed = 10f;
    [SerializeField] private ParticleSystem explosionEffect;
    private GameObject warningArea;
    private Vector3 targetPosition;  // ミサイルの目標座標
    private Vector3 landingPosition;  // 着弾点（警告エリアの位置）
    private bool isTargetReached = false;  // 目標に到達したかどうか

    public void SetTarget(Vector3 target, Vector3 landing, float missileSpeed, GameObject warning)
    {
        targetPosition = target;
        landingPosition = landing;
        speed = missileSpeed;
        warningArea = warning;

        LookAtTarget(targetPosition);
    }

    void Update()
    {
        if (!isTargetReached)
        {
            MoveTowards(targetPosition);

            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                isTargetReached = true;
                StartCoroutine(DropToLanding());
            }
        }
    }

    private IEnumerator DropToLanding()
    {
        LookAtTarget(landingPosition);

        while (Vector3.Distance(transform.position, landingPosition) > 0.1f)
        {
            MoveTowards(landingPosition);
            yield return null;
        }

        Explode();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == warningArea)
        {
            Explode();
        }
    }

    private void Explode()
    {
        if (explosionEffect != null)
        {
            ParticleSystem effectInstance = Instantiate(explosionEffect, landingPosition, Quaternion.identity);
            effectInstance.Play();
            Destroy(effectInstance.gameObject, effectInstance.main.duration);
        }

        if (warningArea != null)
        {
            Destroy(warningArea);
            AudioManager.GetInstance().PlaySound(12);
        }

        Destroy(gameObject);
    }

    /// <summary>
    /// 指定したターゲットを向く
    /// </summary>
    private void LookAtTarget(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(direction);
    }

    /// <summary>
    /// 指定したターゲットへ移動する
    /// </summary>
    private void MoveTowards(Vector3 target)
    {
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
    }
}

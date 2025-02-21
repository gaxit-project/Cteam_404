using UnityEngine;
using System.Collections;

public class NormalLaser : MonoBehaviour
{
    [Header("▽円設定")]
    public Transform centerObject;
    public Transform player;

    [Header("▽レーザー設定")]
    public LineRenderer laserLine;
    public float forwardOffsetAngle = 10f;
    public float laserExtendDistance = 5f;
    public float laserDuration = 2f;

    [Header("▽警告エリア設定")]
    public GameObject warningArea;
    public float warningDuration = 1.5f;

    [Header("▽警告音設定")]
    public AudioClip warningSound;
    private AudioSource audioSource;

    private Vector3 laserStart;
    private Vector3 laserEnd;
    private Collider warningCollider;
    private MeshRenderer warningRenderer;

    void Start()
    {
        audioSource = warningArea.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = warningArea.AddComponent<AudioSource>();
        }
        audioSource.clip = warningSound;

        warningCollider = warningArea.GetComponent<Collider>();
        warningRenderer = warningArea.GetComponent<MeshRenderer>();

        if (warningCollider != null)
        {
            warningCollider.isTrigger = true;
            warningCollider.enabled = false;
        }

        if (warningRenderer != null)
        {
            warningRenderer.enabled = false;
        }
    }

    public void ExecuteAttack()
    {
        // プレイヤーの少し前の座標を計算
        float dynamicRadius = Vector3.Distance(centerObject.position, player.position);
        Vector3 radiusVector = (player.position - centerObject.position).normalized;
        float currentAngle = Mathf.Atan2(radiusVector.z, radiusVector.x) * Mathf.Rad2Deg;
        float targetAngle = currentAngle + forwardOffsetAngle;
        float radians = targetAngle * Mathf.Deg2Rad;

        Vector3 predictedPosition = new Vector3(
            centerObject.position.x + Mathf.Cos(radians) * dynamicRadius,
            player.position.y,
            centerObject.position.z + Mathf.Sin(radians) * dynamicRadius
        );

        // レーザーの開始位置と終点を決定
        laserStart = centerObject.position;
        Vector3 laserDirection = (predictedPosition - laserStart).normalized;
        laserEnd = predictedPosition + (laserDirection * laserExtendDistance);

        Debug.DrawRay(laserStart, laserDirection * laserExtendDistance, Color.green, 5.0f);

        // 警告エリアの設定
        if (warningArea != null)
        {
            float laserLength = Vector3.Distance(laserStart, laserEnd);
            Vector3 warningCenter = (laserStart + laserEnd) / 2;

            warningArea.transform.position = warningCenter;
            warningArea.transform.rotation = Quaternion.LookRotation(laserEnd - laserStart);
            warningArea.transform.localScale = new Vector3(warningArea.transform.localScale.x, warningArea.transform.localScale.y, laserLength);

            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }

            if (warningRenderer != null)
            {
                warningRenderer.enabled = true;
            }

            if (warningCollider != null)
            {
                warningCollider.enabled = true;
            }
        }

        StartCoroutine(LaserWarningCoroutine());
    }

    private IEnumerator LaserWarningCoroutine()
    {
        yield return new WaitForSeconds(warningDuration);

        // レーザー発射の瞬間に警告エリアのコライダー内にいるプレイヤーを判定
        ApplyLaserDamage();

        // 警告エリアの見た目だけ消す（コライダーは残す）
        if (warningRenderer != null)
        {
            warningRenderer.enabled = false;
        }

        // レーザー発射
        laserLine.SetPosition(0, laserStart);
        laserLine.SetPosition(1, laserEnd);
        laserLine.enabled = true;

        yield return new WaitForSeconds(laserDuration);

        // レーザーが消えるタイミングで警告エリアのコライダーも無効化
        laserLine.enabled = false;
        if (warningCollider != null)
        {
            warningCollider.enabled = false;
        }
    }

    private void ApplyLaserDamage()
    {
        if (player == null) return;

        Collider playerCollider = player.GetComponent<Collider>();
        if (playerCollider == null || warningCollider == null) return;

        if (warningCollider.bounds.Intersects(playerCollider.bounds))
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage();
            }
        }
    }
}

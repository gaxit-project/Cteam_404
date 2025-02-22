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
    public LayerMask playerLayer;

    [Header("▽警告エリア設定")]
    public GameObject warningArea;
    public float warningDuration = 1.5f;

    [Header("▽警告音設定")]
    public AudioClip warningSound;
    private AudioSource audioSource;

    private Vector3 laserStart;
    private Vector3 laserEnd;
    private bool isLaserActive = false;
    private Collider warningCollider;

    void Start()
    {
        audioSource = warningArea.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = warningArea.AddComponent<AudioSource>();
        }
        audioSource.clip = warningSound;

        warningCollider = warningArea.GetComponent<Collider>();
        if (warningCollider == null)
        {
            Debug.LogError("警告エリアに Collider がありません。");
        }
        warningCollider.enabled = false; // 初期状態では無効化
        warningCollider.isTrigger = true; // トリガーとして扱う
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

        // レーザーの開始位置と終了位置を設定
        laserStart = centerObject.position;
        Vector3 laserDirection = (predictedPosition - laserStart).normalized;
        laserEnd = predictedPosition + (laserDirection * laserExtendDistance);

        Debug.DrawRay(laserStart, laserDirection * laserExtendDistance, Color.green, 5.0f);

        // 警告エリアを設定
        if (warningArea != null)
        {
            float laserLength = Vector3.Distance(laserStart, laserEnd);
            Vector3 warningCenter = (laserStart + laserEnd) / 2;

            warningArea.transform.position = warningCenter;
            Quaternion rotation = Quaternion.LookRotation(laserEnd - laserStart);
            warningArea.transform.rotation = rotation;
            warningArea.transform.localScale = new Vector3(
                warningArea.transform.localScale.x,
                warningArea.transform.localScale.y,
                laserLength
            );

            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }

            warningArea.SetActive(true);
            warningCollider.enabled = false; // 警告時は当たり判定なし
        }

        StartCoroutine(LaserWarningCoroutine());
    }

    private IEnumerator LaserWarningCoroutine()
    {
        float elapsedTime = 0f;
        Renderer warningRenderer = warningArea.GetComponent<Renderer>();
        Material warningMaterial = warningRenderer.material;
        Color initialColor = warningMaterial.color;

        // 警告エリアを点滅
        while (elapsedTime < warningDuration)
        {
            float alpha = Mathf.PingPong(elapsedTime * 5f, 1f);
            warningMaterial.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        warningMaterial.color = new Color(initialColor.r, initialColor.g, initialColor.b, 1f);

        yield return new WaitForSeconds(warningDuration);

        // レーザー発射
        laserLine.enabled = true;
        laserLine.SetPosition(0, laserStart);
        laserLine.SetPosition(1, laserEnd);
        isLaserActive = true;

        // 警告エリアの視覚を消し、当たり判定のみを残す
        warningRenderer.enabled = false;
        warningCollider.enabled = true; // 当たり判定を有効化
        warningCollider.isTrigger = true;

        // レーザーの持続時間後に消す
        yield return new WaitForSeconds(laserDuration);
        laserLine.enabled = false;
        isLaserActive = false;
        warningCollider.enabled = false; // 当たり判定も無効化
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isLaserActive) return; // レーザーが発射されていない場合は無効

        if (other.gameObject == player.gameObject)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage();
                Debug.Log("Playerがレーザーダメージを受けた");
            }
        }
    }
}

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
    public float laserDuration = 3f;

    [Header("▽警告エリア設定")]
    public GameObject warningAreaPrefab;
    private GameObject warningAreaInstance;
    public float warningDuration = 1.5f;

    [Header("▽警告音設定")]
    public AudioClip warningSound;
    private AudioSource audioSource;

    [Header("▽LineWallSetup設定")]
    [SerializeField] private LineWallSetup lineWallSetup;

    private Vector3 laserStart;
    private Vector3 laserEnd;
    private bool isLaserActive = false;
    private BoxCollider warningCollider;
    private Renderer warningRenderer;

    void Start()
    {
        if (lineWallSetup == null)
        {
            lineWallSetup = GetComponent<LineWallSetup>();
        }
    }

    public void ExecuteAttack()
    {
        //プレイヤーの少し前の座標を計算
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

        //レーザーの開始位置と終了位置を設定
        laserStart = centerObject.position;
        Vector3 laserDirection = (predictedPosition - laserStart).normalized;
        laserEnd = predictedPosition + (laserDirection * laserExtendDistance);

        Debug.DrawRay(laserStart, laserDirection * laserExtendDistance, Color.green, 5.0f);

        //警告エリアをインスタンス化
        if (warningAreaPrefab != null)
        {
            float laserLength = Vector3.Distance(laserStart, laserEnd);
            Vector3 warningCenter = (laserStart + laserEnd) / 2;

            warningAreaInstance = Instantiate(warningAreaPrefab, warningCenter, Quaternion.identity, transform);
            warningCollider = warningAreaInstance.GetComponent<BoxCollider>();
            warningRenderer = warningAreaInstance.GetComponent<Renderer>();
            audioSource = warningAreaInstance.GetComponent<AudioSource>();

            if (warningCollider != null)
            {
                warningCollider.isTrigger = true;
                warningCollider.enabled = false; //警告エリアのコライダーは最初無効
                warningCollider.size = new Vector3(0.1f, 5f, laserLength); //コライダーのサイズをレーザーの長さに合わせる
            }

            if (warningRenderer != null)
            {
                warningRenderer.enabled = true;
            }

            if (audioSource != null && warningSound != null)
            {
                audioSource.clip = warningSound;
                audioSource.Play();
            }

            warningAreaInstance.transform.localScale = new Vector3(1f, 1f, laserLength);
            Quaternion rotation = Quaternion.LookRotation(laserEnd - laserStart);
            warningAreaInstance.transform.rotation = rotation;

            if (lineWallSetup != null)
            {
                lineWallSetup.enabled = true;
                lineWallSetup.SetupWalls();
            }
        }

        StartCoroutine(LaserWarningCoroutine());
    }

    private IEnumerator LaserWarningCoroutine()
    {
        float elapsedTime = 0f;
        Material warningMaterial = warningRenderer.material;
        Color initialColor = warningMaterial.color;

        while (elapsedTime < warningDuration)
        {
            float alpha = Mathf.PingPong(elapsedTime * 5f, 1f);
            warningMaterial.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (audioSource != null && warningSound != null)
        {
            audioSource.Play();
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

        // レーザーの持続時間後に消す
        yield return StartCoroutine(WaitForSecondsWithCheck(laserDuration));
        laserLine.enabled = false;
        isLaserActive = false;
        warningCollider.enabled = false; // 当たり判定を無効化
        Destroy(warningAreaInstance); // 警告エリアを完全に削除

        // LineWallSetupをオフ（laserDuration終了後に無効化）
        if (lineWallSetup != null)
        {
            lineWallSetup.enabled = false;
            lineWallSetup.CleanupWalls();
        }
    }

    private IEnumerator WaitForSecondsWithCheck(float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            if (Time.timeScale == 0) yield return new WaitUntil(() => Time.timeScale > 0);
            yield return null;
        }
    }

    // 警告エリアにプレイヤーが触れたときの処理
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && isLaserActive && !other.gameObject.CompareTag("WarningArea"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage();
            }
        }
    }
}
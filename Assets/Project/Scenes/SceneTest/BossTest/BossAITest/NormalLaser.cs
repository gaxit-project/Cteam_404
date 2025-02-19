using UnityEngine;
using System.Collections;

public class NormalLaser : MonoBehaviour
{
    [Header("▽円設定")]
    [Header("円の中心")]
    public Transform centerObject;
    [Header("プレイヤー")]
    public Transform player;

    [Header("▽レーザー設定")]
    [Header("ラインレンダー")]
    public LineRenderer laserLine;
    [Header("レーザー発射角度(度数法)")]
    public float forwardOffsetAngle = 10f;
    [Header("ラインレンダー延長距離")]
    public float laserExtendDistance = 5f;
    [InspectorName("レーザーの持続時間")]
    public float laserDuration = 2f;
    [Header("Rayの太さ")]
    public float laserRadius = 50f;
    [Header("プレイヤーのLayer")]
    public LayerMask playerLayer;

    [Header("▽警告エリア設定")]
    [Header("警告エリア (Quad)")]
    public GameObject warningArea;
    [Header("警告表示時間")]
    public float warningDuration = 1.5f;

    [Header("▽警告音設定")]
    [Header("警告音")]
    public AudioClip warningSound;
    private AudioSource audioSource;

    private Vector3 laserStart;
    private Vector3 laserEnd;
    private bool isLaserActive = false;

    void Start()
    {
        audioSource = warningArea.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = warningArea.AddComponent<AudioSource>();
        }
        audioSource.clip = warningSound;
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

        //Rayを発射してレーザーの着弾地点を決定
        laserStart = centerObject.position;
        Vector3 laserDirection = (predictedPosition - laserStart).normalized;
        laserEnd = predictedPosition + (laserDirection * laserExtendDistance);

        Debug.DrawRay(laserStart, laserDirection * laserExtendDistance, Color.green, 5.0f);

        //QuadをRayの長さに合わせて伸ばす
        if (warningArea != null)
        {
            float laserLength = Vector3.Distance(laserStart, laserEnd);
            Vector3 warningCenter = (laserStart + laserEnd) / 2;

            warningArea.transform.position = warningCenter;

            // 終点方向を向ける（回転の調整）
            Vector3 direction = (laserEnd - laserStart).normalized;
            Quaternion rotation = Quaternion.LookRotation(direction);
            warningArea.transform.rotation = rotation;

            // 長さを調整（Z軸方向に伸ばす）
            warningArea.transform.localScale = new Vector3(warningArea.transform.localScale.x, warningArea.transform.localScale.y, laserLength);

            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }


            warningArea.SetActive(true);
        }

        StartCoroutine(LaserWarningCoroutine());
    }

    private IEnumerator LaserWarningCoroutine()
    {
        float elapsedTime = 0f;
        Renderer warningRenderer = warningArea.GetComponent<Renderer>();
        Material warningMaterial = warningRenderer.material;
        Color initialColor = warningMaterial.color;

        // 警告エリアを表示する時間
        while (elapsedTime < warningDuration)
        {
            float alpha = Mathf.PingPong(elapsedTime * 5f, 1f);  // 5fは点滅の速さ
            Color newColor = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);
            warningMaterial.color = newColor;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 最終的な透明度を1にして完全に表示
        warningMaterial.color = new Color(initialColor.r, initialColor.g, initialColor.b, 1f);

        // 警告エリアを非表示にするタイミングを変更
        // 警告が終わった直後ではなく、レーザーが発射される直前に非表示にする
        yield return new WaitForSeconds(warningDuration);  // 警告エリアの表示時間を待つ

        // レーザー発射時に警告エリアを消す
        warningArea.SetActive(false); // ラインレンダーが表示されるタイミングで消す

        // レーザー発射
        laserLine.SetPosition(0, laserStart);
        laserLine.SetPosition(1, laserEnd);
        laserLine.enabled = true;
        isLaserActive = true;

        // ダメージ判定
        CheckLaserHit(laserStart, laserEnd);

        // レーザーの持続時間後に消す
        yield return new WaitForSeconds(laserDuration);
        laserLine.enabled = false;
        isLaserActive = false;
    }

    private void CheckLaserHit(Vector3 start, Vector3 end)
    {
        if (player == null) return;

        Vector3 direction = (end - start).normalized;
        float distance = Vector3.Distance(start, end);
        Debug.DrawRay(start, direction * distance, Color.green, 5.0f);

        RaycastHit[] hits = Physics.SphereCastAll(start, laserRadius, direction, distance, playerLayer);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.CompareTag("Player"))
            {
                PlayerHealth playerHealth = hit.collider.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage();
                    Debug.Log("Playerがレーザーダメージを受けた");
                }
            }
        }
    }
}
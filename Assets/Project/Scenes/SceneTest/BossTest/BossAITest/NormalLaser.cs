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
    private BoxCollider warningCollider;
    private MeshRenderer warningRenderer;

    void Start()
    {
        audioSource = warningArea.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = warningArea.AddComponent<AudioSource>();
        }
        audioSource.clip = warningSound;

        // 警告エリアのコライダーとメッシュレンダーを取得（最初は非表示）
        warningCollider = warningArea.GetComponent<BoxCollider>();
        warningRenderer = warningArea.GetComponent<MeshRenderer>();

        if (warningCollider != null)
        {
            warningCollider.isTrigger = true; // トリガーとして設定
            warningCollider.enabled = false;  // 警告エリアのコライダーは最初無効
        }

        if (warningRenderer != null)
        {
            warningRenderer.enabled = false; // 初期状態では非表示
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
            warningArea.transform.localScale = Vector3.one; // スケールを(1,1,1)にリセット
            warningCollider.size = new Vector3(1f, 5f, laserLength); // Yサイズを5に変更（プレイヤーの高さに合わせる）

            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }

            // メッシュレンダーをONにして可視化
            if (warningRenderer != null)
            {
                warningRenderer.enabled = true;
            }

            // コライダーを有効化（ビームが消えるまで残す）
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

        // 警告エリアの見た目を維持（必要に応じてコメントアウト）
        // if (warningRenderer != null)
        // {
        //     warningRenderer.enabled = false;
        // }

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
        if (warningRenderer != null)
        {
            warningRenderer.enabled = false; // レーザー終了後に非表示
        }
    }

    // 警告エリアにプレイヤーが触れたときの処理
    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"OnTriggerEnter が呼ばれました: {other.gameObject.name}, Tag: {other.tag}, Position: {other.transform.position}, Collider: {other.GetType().Name}, Is Trigger: {other.isTrigger}");

        // warningArea自身を除外し、プレイヤーのみを検知
        if (other.CompareTag("Player") && other.gameObject != warningArea)
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage();
                Debug.Log("Playerがレーザーダメージを受けた");
            }
            else
            {
                Debug.LogError("PlayerHealth component not found on player!");
            }
        }
    }

    // デバッグ用：BoxColliderの範囲をGizmosで表示
    private void OnDrawGizmos()
    {
        if (warningCollider != null && warningCollider.enabled)
        {
            Gizmos.color = Color.red;
            Gizmos.matrix = warningCollider.transform.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero, warningCollider.size);
        }
    }
}
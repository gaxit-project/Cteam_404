using UnityEngine;
using SplineMesh;
using System.Collections;

public class MissileAttack : MonoBehaviour
{
    [Header("ビーム設定（NormalLaserを使用）")]
    [SerializeField] private NormalLaser normalLaser;
    [SerializeField] private float pushBackDistance = 5f;
    [SerializeField] private float pushBackDuration = 0.5f;

    [Header("ミサイル設定")]
    [SerializeField] private GameObject missilePrefab;
    [SerializeField] private float missileLaunchDelay = 1f;
    [SerializeField] private float missileSpeed = 10f;
    [SerializeField] private float dropSpeed = 5f;
    [SerializeField] private float explosionRadius = 2f;

    [Header("着弾点設定")]
    [SerializeField] private float offsetDistance1 = 5f;
    [SerializeField] private float offsetDistance2 = 10f;

    [Header("プレイヤー設定")]
    [SerializeField] private Player player;
    [SerializeField] private float playerBoostSpeed = 15f;
    [SerializeField] private float playerBoostDuration = 2f;

    [Header("ミサイルターゲット設定")]
    [SerializeField] private float targetHeight = 5f;

    [Header("警告エリア設定")]
    [SerializeField] private GameObject warningAreaPrefab;
    [SerializeField] private float warningDuration = 1.5f;
    [SerializeField] private AudioClip warningSound;

    private GameObject[] warningAreas;
    private RailManager railManager;

    void Start()
    {
        if (player == null)
        {
            player = FindObjectOfType<Player>();
        }
        if (normalLaser == null)
        {
            normalLaser = GetComponent<NormalLaser>();
        }
        railManager = FindObjectOfType<RailManager>();
    }

    public void ExecuteAttack()
    {
        if (player == null || normalLaser == null || missilePrefab == null || warningAreaPrefab == null || railManager == null) return;

        StartCoroutine(PerformAttack());
    }

    private IEnumerator PerformAttack()
    {
        //NormalLaserのビームを発射
        normalLaser.ExecuteAttack();

        yield return new WaitForSeconds(normalLaser.laserDuration + pushBackDuration);

        // プレイヤーの現在位置を取得
        Vector3 playerPos = player.transform.position;

        Vector3 bossPos = transform.position;
        Vector3 beamDirection = (playerPos - bossPos).normalized;
        Vector3 pushBackDirection = -beamDirection;
        Vector3 targetPosition = player.transform.position + (pushBackDirection * pushBackDistance);

        float elapsedTime = 0f;
        Vector3 initialPosition = player.transform.position;
        while (elapsedTime < pushBackDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / pushBackDuration;
            player.transform.position = Vector3.Lerp(initialPosition, targetPosition, t);
            yield return null;
        }

        //ミサイル発射の遅延
        yield return new WaitForSeconds(missileLaunchDelay);

        //プレイヤーの平均速度を取得（Speedを仮定）
        float playerAverageSpeed = player.Speed;
        if (playerAverageSpeed <= 0) playerAverageSpeed = 10f;

        //RailManagerからプレイヤーのレール位置を取得
        int nearIndex = railManager.GetNearPositionIndex(player.transform.position);
        float railPosition = railManager.GetNearRailPosition(nearIndex);
        if (railPosition < 0) railPosition = 0f;

        //ミサイルの着弾点を計算（レール上の位置と前方）
        Vector3 currentPlayerPos = player.transform.position;
        Vector3 railDirection = GetRailDirectionFromRailManager(railPosition);
        Vector3 impactPoint1 = currentPlayerPos;
        Vector3 impactPoint2 = currentPlayerPos + (railDirection * offsetDistance1 / playerAverageSpeed);
        Vector3 impactPoint3 = currentPlayerPos + (railDirection * offsetDistance2 / playerAverageSpeed);

        //警告エリアを表示
        warningAreas = new GameObject[3];
        StartCoroutine(ShowWarningAreas(impactPoint1, impactPoint2, impactPoint3));

        //ミサイルを発射（3発）
        LaunchMissile(impactPoint1);
        yield return new WaitForSeconds(0.3f);
        LaunchMissile(impactPoint2);
        yield return new WaitForSeconds(0.3f);
        LaunchMissile(impactPoint3);

    }

    private Vector3 GetRailDirectionFromRailManager(float railPosition)
    {
        if (railManager != null && railManager.TargetRail != null)
        {
            float nextPosition = railPosition + 0.01f;
            if (nextPosition > 1f) nextPosition -= 1f;
            Vector3 nextPoint = railManager.TargetRail.GetSample(nextPosition).location;
            Vector3 currentPoint = railManager.TargetRail.GetSample(railPosition).location;
            return (nextPoint - currentPoint).normalized;
        }
        return Vector3.forward;
    }

    private IEnumerator ShowWarningAreas(Vector3 point1, Vector3 point2, Vector3 point3)
    {
        // 3つの着陸点に警告エリアを表示
        warningAreas[0] = InstantiateWarningArea(point1);
        warningAreas[1] = InstantiateWarningArea(point2);
        warningAreas[2] = InstantiateWarningArea(point3);

        yield return new WaitForSeconds(warningDuration);

        foreach (GameObject area in warningAreas)
        {
            if (area != null)
            {
                Destroy(area);
            }
        }
    }

    private GameObject InstantiateWarningArea(Vector3 position)
    {
        GameObject warningArea = Instantiate(warningAreaPrefab, position, Quaternion.identity);
        BoxCollider collider = warningArea.GetComponent<BoxCollider>();
        Renderer renderer = warningArea.GetComponent<Renderer>();
        AudioSource audioSource = warningArea.GetComponent<AudioSource>();

        if (collider != null)
        {
            collider.isTrigger = true;
            collider.size = new Vector3(explosionRadius * 2, 5f, explosionRadius * 2);
        }

        if (renderer != null)
        {
            renderer.enabled = true;
            Material material = renderer.material;
            StartCoroutine(BlinkWarningArea(material));
        }

        if (audioSource != null && warningSound != null)
        {
            audioSource.clip = warningSound;
            audioSource.Play();
        }

        return warningArea;
    }

    private IEnumerator BlinkWarningArea(Material material)
    {
        float elapsedTime = 0f;
        Color initialColor = material.color;

        while (elapsedTime < warningDuration)
        {
            float alpha = Mathf.PingPong(elapsedTime * 5f, 1f);
            material.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        material.color = new Color(initialColor.r, initialColor.g, initialColor.b, 1f);
    }

    private void LaunchMissile(Vector3 impactPoint)
    {
        Vector3 playerPos = player.transform.position;
        Vector3 targetPos = playerPos + Vector3.up * targetHeight;

        Vector3 launchPosition = normalLaser.centerObject.position;

        GameObject missile = Instantiate(missilePrefab, launchPosition, Quaternion.identity);
        Missile missileScript = missile.GetComponent<Missile>();
        if (missileScript != null)
        {
            missileScript.SetTarget(targetPos, impactPoint, missileSpeed, dropSpeed, explosionRadius);
        }
    }
}
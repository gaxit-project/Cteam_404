using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NormalLaser : MonoBehaviour
{
    [Header("���~�ݒ�")]
    public Transform centerObject;
    public Transform player;

    [Header("�����[�U�[�ݒ�")]
    public LineRenderer laserLine;
    public float forwardOffsetAngle = 10f;
    public float laserExtendDistance = 5f;
    public float laserDuration = 3f; // 3�b�ɕύX

    [Header("���x���G���A�ݒ�")]
    public GameObject warningAreaPrefab; // �v���n�u�Ƃ��Čx���G���A��w��
    private GameObject warningAreaInstance; // �C���X�^���X�����ꂽ�x���G���A
    public float warningDuration = 1.5f;

    [Header("���x�����ݒ�")]
    public AudioClip warningSound;
    private AudioSource audioSource;

    [Header("��LineWallSetup�ݒ�")]
    [SerializeField] private LineWallSetup lineWallSetup; // LineWallSetup�̎Q��

    private BossStateAI bossStateAI;

    private Vector3 laserStart;
    private Vector3 laserEnd;
    private bool isLaserActive = false;
    private BoxCollider warningCollider;
    private Renderer warningRenderer;

    private List<GameObject> activeWarningAreas = new List<GameObject>();
    private List<BoxCollider> activeWarningColliders = new List<BoxCollider>();

    void Start()
    {
        if (lineWallSetup == null)
        {
            lineWallSetup = GetComponent<LineWallSetup>();
        }

        if (bossStateAI == null)
        {
            bossStateAI = FindObjectOfType<BossStateAI>();
        }
    }

    public void ExecuteAttack()
    {
        BossFace.Instance.ChangeFace(5);
        StartCoroutine(ResetFaceAfterDelay(0.7f));

        // プレイヤーの少し前の座標を計算
        float dynamicRadius = Vector3.Distance(centerObject.position, player.position);
        Vector3 radiusVector = (player.position - centerObject.position).normalized;
        float currentAngle = Mathf.Atan2(radiusVector.z, radiusVector.x) * Mathf.Rad2Deg;
        float noiseValue = Mathf.PerlinNoise(player.position.x, player.position.z);
        float targetAngle = currentAngle + forwardOffsetAngle * noiseValue;
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

        if (warningAreaPrefab != null)
        {
            float laserLength = Vector3.Distance(laserStart, laserEnd);
            Vector3 warningCenter = (laserStart + laserEnd) / 2;

            warningAreaInstance = Instantiate(warningAreaPrefab, warningCenter, Quaternion.identity, transform);
            warningCollider = warningAreaInstance.GetComponent<BoxCollider>();
            warningRenderer = warningAreaInstance.GetComponent<Renderer>();

            activeWarningAreas.Add(warningAreaInstance);
            activeWarningColliders.Add(warningCollider);

            if (warningCollider != null)
            {
                warningCollider.isTrigger = true;
                warningCollider.enabled = false;
                warningCollider.size = new Vector3(0.1f, 5f, laserLength);
            }

            if (warningRenderer != null)
            {
                warningRenderer.enabled = true;
            }

            warningAreaInstance.transform.localScale = new Vector3(1f, 1f, laserLength);
            warningAreaInstance.transform.rotation = Quaternion.LookRotation(laserEnd - laserStart);

            if (lineWallSetup != null)
            {
                lineWallSetup.enabled = true;
                lineWallSetup.SetupWalls();
            }
        }

        AudioManager.GetInstance().PlaySound(15);

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

        warningMaterial.color = new Color(initialColor.r, initialColor.g, initialColor.b, 1f);
        yield return new WaitForSeconds(warningDuration);

        // レーザー発射
        laserLine.enabled = true;
        laserLine.positionCount = 2;
        laserLine.SetPosition(0, laserStart);
        laserLine.SetPosition(1, laserEnd);
        isLaserActive = true;



        //AudioManager.GetInstance().PlaySound();



        // 警告エリアの視覚を消し、当たり判定のみを残す
        if (warningRenderer != null)
            warningRenderer.enabled = false;

        if (warningCollider != null)
            warningCollider.enabled = true;

        yield return StartCoroutine(WaitForSecondsWithCheck(laserDuration));

        isLaserActive = false;
        laserLine.enabled = false;

        if (warningAreaInstance != null)
        {
            activeWarningAreas.Remove(warningAreaInstance);
            activeWarningColliders.Remove(warningCollider);
            Destroy(warningAreaInstance);
        }

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

    private void OnTriggerEnter(Collider other)
    {
        if (isLaserActive && other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage();
            }
        }
    }

    private IEnumerator ResetFaceAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        bossStateAI.BossFacePhase();
    }
}
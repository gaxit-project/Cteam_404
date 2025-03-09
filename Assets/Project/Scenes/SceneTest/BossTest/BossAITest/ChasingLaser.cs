using System.Collections;
using UnityEngine;

public class ChasingLaser : MonoBehaviour
{
    [Header("レーザー設定")]
    public Transform bossCenter;
    public Transform player;
    public GameObject laserPrefab;
    public float laserDuration = 3f;
    public float stretchDuration = 1f; // 伸びるのにかかる時間
    public float backOffsetDistance = 5f; // プレイヤー後方のレーザーの初期座標
    public float chaseSpeed = 5f; // 追跡速度
    public float laserExtendLength = 2f; // レーザーの延長
    public float fixedY = 1.0f; // Y座標の固定値

    private GameObject laserInstance;
    private Vector3 targetPos;
    private bool isLaserActive = false;

    public void ExecuteAttack()
    {
        StartCoroutine(FireLaser());
    }

    private IEnumerator FireLaser()
    {
        laserInstance = Instantiate(laserPrefab, bossCenter.position, Quaternion.identity);
        isLaserActive = true;

        Vector3 playerDirection = (player.position - bossCenter.position).normalized;
        Vector3 finalTargetPos = player.position - playerDirection * backOffsetDistance;
        finalTargetPos.y = fixedY;

        Vector3 startPos = bossCenter.position;
        startPos.y = fixedY;

        float stretchTime = 0f;
        while (stretchTime < stretchDuration)
        {
            if (laserInstance == null) yield break;

            stretchTime += Time.deltaTime;
            float stretchProgress = stretchTime / stretchDuration;
            targetPos = Vector3.Lerp(startPos, finalTargetPos, stretchProgress);
            UpdateLaserTransform(startPos, targetPos);

            yield return null;
        }

        targetPos = finalTargetPos;
        UpdateLaserTransform(startPos, targetPos);

        float chaseTime = laserDuration - stretchDuration;
        float elapsedChaseTime = 0f;

        while (elapsedChaseTime < chaseTime)
        {
            if (laserInstance == null) yield break;

            elapsedChaseTime += Time.deltaTime;

            startPos = bossCenter.position;
            startPos.y = fixedY;

            targetPos = Vector3.MoveTowards(targetPos, player.position, chaseSpeed * Time.deltaTime);
            targetPos.y = fixedY;

            UpdateLaserTransform(startPos, targetPos);

            yield return null;
        }

        if (laserInstance != null)
        {
            Destroy(laserInstance);
            isLaserActive = false;
        }
    }

    private void UpdateLaserTransform(Vector3 startPos, Vector3 endPos)
    {
        if (!isLaserActive || laserInstance == null) return;

        // レーザーの方向と長さ
        Vector3 laserDirection = (endPos - startPos).normalized;
        float laserLength = Vector3.Distance(startPos, endPos) + laserExtendLength;

        // レーザーの位置・スケール・向きを設定
        laserInstance.transform.position = (startPos + endPos) / 2;
        laserInstance.transform.rotation = Quaternion.LookRotation(laserDirection);
        laserInstance.transform.localScale = new Vector3(0.5f, 0.5f, laserLength);
    }
}
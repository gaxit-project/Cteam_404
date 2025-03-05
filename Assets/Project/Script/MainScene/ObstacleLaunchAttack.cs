using UnityEngine;

public class ObstacleLaunchAttack : MonoBehaviour
{
    [Header("障害物の設定")]
    [SerializeField] private GameObject obstaclePrefab;      //障害物のプレハブ
    [SerializeField] private float spawnOffsetY = -10f;     //生成位置
    [SerializeField] private float moveDuration = 0.5f;     //画面外からBossとPlayerの中間まで移動する時間
    [SerializeField] private float pauseDuration = 1f;      //停止する時間
    [SerializeField] private float launchSpeed = 10f;       //飛ばす速度
    [SerializeField] private float launchDuration = 3f;     //飛ばす時間
    [SerializeField] private GameObject bossCenter;         //ボスの中心

    private Player player;                                  //プレイヤー参照
    private GameObject spawnedObstacle;                     //生成した障害物

    private float timer = 0f;
    private Vector3 startPos;                               //生成時の位置
    private Vector3 targetPos;                              //プレイヤーとボスの中間位置
    private enum Phase { Moving, Paused, Launching }
    private Phase currentPhase = Phase.Moving;

    private void Start()
    {
        if (bossCenter == null)
        {
            bossCenter = transform.Find("BossCenter")?.gameObject;
        }
    }

    public void ExecuteAttack()
    {
        if (bossCenter == null || player == null || obstaclePrefab == null)
        {
            return;
        }

        Vector3 playerPos = player.transform.position;
        Vector3 bossCenterPos = bossCenter.transform.position;
        Vector3 midPoint = (playerPos + bossCenterPos) * 0.5f;

        startPos = new Vector3(midPoint.x, midPoint.y + spawnOffsetY, midPoint.z);
        targetPos = midPoint;

        spawnedObstacle = Instantiate(obstaclePrefab, startPos, Quaternion.identity);
        spawnedObstacle.transform.up = Vector3.up;

        timer = 0f;
        currentPhase = Phase.Moving;

        StartCoroutine(UpdateObstacle());
    }

    private System.Collections.IEnumerator UpdateObstacle()
    {
        while (spawnedObstacle != null)
        {
            timer += Time.deltaTime;

            switch (currentPhase)
            {
                case Phase.Moving:
                    // 画面外からBossとPlayerの中間まで移動
                    float moveProgress = Mathf.Clamp01(timer / moveDuration);
                    spawnedObstacle.transform.position = Vector3.Lerp(startPos, targetPos, moveProgress);

                    if (moveProgress >= 1f)
                    {
                        currentPhase = Phase.Paused;
                        timer = 0f;
                    }
                    break;

                case Phase.Paused:
                    Vector3 playerPos = player.transform.position;
                    Vector3 bossCenterPos = bossCenter.transform.position;
                    Vector3 midPoint = (playerPos + bossCenterPos) * 0.5f;
                    spawnedObstacle.transform.position = new Vector3(midPoint.x, targetPos.y, midPoint.z);

                    if (timer >= pauseDuration)
                    {
                        currentPhase = Phase.Launching;
                        timer = 0f;
                    }
                    break;

                case Phase.Launching:
                    Vector3 direction = (spawnedObstacle.transform.position - bossCenter.transform.position).normalized;
                    spawnedObstacle.transform.position += direction * launchSpeed * Time.deltaTime;

                    if (timer >= launchDuration)
                    {
                        Destroy(spawnedObstacle);
                        break;
                    }
                    break;
            }

            yield return null;
        }

        BossStateAI bossAI = GetComponentInParent<BossStateAI>();
        if (bossAI != null)
        {
            bossAI.SpecialAttackFinished();
        }
    }
}
using UnityEngine;
using SplineMesh;

public class MobGenerate : MonoBehaviour
{
    [Header("対象のレール")]
    public Spline TargetRail; // 管理対象のレール

    [Header("MobEnemyオブジェクト")]
    [SerializeField] private GameObject MobEnemy; // レール上に配置する参照用オブジェクトのプレハブ

    [Header("プレイヤーオブジェクト")]
    [SerializeField] private GameObject player; // プレイヤーオブジェクト

    [Header("探知距離")]
    [SerializeField] private float _distance = 10f; // プレイヤーとの最大探知距離

    [Header("生成間隔 (秒)")]
    [SerializeField] private float spawnInterval = 5f; // 生成間隔

    private float _timer;

    private void Start()
    {
        _timer = spawnInterval;
    }

    private void Update()
    {
        if (PlayerMove.isRide)
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0f)
            {
                Generate();
                _timer = spawnInterval; // タイマーをリセット
                Debug.Log("Gen");
            }
        }
    }

    private void Generate()
    {
        RailManager[] railManagers = FindObjectsOfType<RailManager>();
        if (railManagers.Length == 0) return;
        Debug.Log("Gen1");

        // ランダムにレールを選択
        int rnd = Random.Range(0, railManagers.Length);
        RailManager selectedRail = railManagers[rnd];
        TargetRail = selectedRail.TargetRail;

        // プレイヤーに最も近いインデックスを取得
        int nearestIndex = selectedRail.GetNearPositionIndex(player.transform.position);
        if (nearestIndex == -1) return; // 有効な位置がない場合は終了
        Debug.Log("Gen2");

        // 最近傍のスプライン上の位置を取得
        float nearestDistance = selectedRail.GetNearRailPosition(nearestIndex);
        var sample = TargetRail.GetSampleAtDistance(nearestDistance);
        Debug.Log("Gen3");

        // プレイヤーとその位置間の距離を計算
        float distanceToPlayer = Vector3.Distance(player.transform.position, sample.location);
        //if (distanceToPlayer > _distance) return; // プレイヤーとの距離が探知範囲外なら生成しない
        Debug.Log("Gen4");

        // オブジェクトを生成して配置
        GameObject enemyObject = Instantiate(MobEnemy, sample.location, Quaternion.identity);

        Debug.Log("せいせい");

        // プレイヤーの方向を向かせる
        Vector3 toPlayer = player.transform.position - sample.location;
        enemyObject.transform.rotation = Quaternion.LookRotation(toPlayer.normalized);
    }
}

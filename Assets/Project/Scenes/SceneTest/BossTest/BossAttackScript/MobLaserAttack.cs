using System.Collections;
using UnityEngine;
using SplineMesh;

public class MobLaserAttack : MonoBehaviour, IBossAttack
{
    [Header("攻撃準備時間")]
    [SerializeField] private float prepareTime = 2f;

    [Header("ビーム発射後の消滅時間")]
    [SerializeField] private float disappearTime = 3f;

    [Header("出現位置オフセット")]
    [SerializeField] private float spawnHeight = 5f; // プレイヤーの上に出現する高さ
    [SerializeField] private float forwardOffset = 3f; // プレイヤーの前に出現する距離
    [SerializeField] private float fadeInDuration = 0.2f; // フェードイン時間

    private GameObject player;
    private bool isPreparing = false;
    private bool isAttacking = false;
    private Spline currentRail; // 現在のレール（追従用）
    private Vector3 spawnPosition; // スポーン位置を記録

    public void ExecuteAttack(GameObject boss)
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        // プレイヤーの上かつ少し前にスポーン
        spawnPosition = player.transform.position + Vector3.up * spawnHeight + player.transform.forward * forwardOffset;
        transform.position = spawnPosition;

        StartCoroutine(AttackSequence());
    }

    private IEnumerator AttackSequence()
    {
        // 上から下にフェードイン
        yield return FadeInToPosition();

        // プレイヤーのレールを追従（最初に降り立ったレールを取得）
        yield return new WaitForSeconds(prepareTime);

        // 攻撃準備開始（レール移動を無視）
        isPreparing = true;

        // 数秒後にビーム発射
        yield return new WaitForSeconds(prepareTime);
        FireBeam();

        // ビーム発射後フェードアウト
        yield return new WaitForSeconds(disappearTime);
        yield return FadeOut();

        Destroy(gameObject);
    }

    private void FireBeam()
    {
        Debug.Log("モブがビームを発射！");
        // ビーム発射ロジックを追加
    }

    private IEnumerator FadeInToPosition()
    {
        float elapsedTime = 0f;
        Vector3 start = transform.position;
        Vector3 target = player.transform.position + player.transform.forward * forwardOffset; // プレイヤーの前方へ移動

        // フェードイン中、プレイヤーから一定の距離を保ちながら移動
        while (elapsedTime < fadeInDuration)
        {
            // プレイヤーから一定の距離を保つ
            Vector3 direction = (target - player.transform.position).normalized;
            transform.position = Vector3.Lerp(start, target, elapsedTime / fadeInDuration);
            transform.position = player.transform.position + direction * forwardOffset;  // プレイヤーから一定の距離

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = player.transform.position + (target - player.transform.position).normalized * forwardOffset;
    }

    private IEnumerator FadeOut()
    {
        Debug.Log("モブがフェードアウト");
        yield return new WaitForSeconds(1f);
    }

    private void Update()
    {
        if (player == null) return;

        // 攻撃準備が始まったらレールの追尾を停止
        if (isPreparing)
        {
            // 攻撃準備段階ではプレイヤーと同じレールに追従
            transform.position = player.transform.position + player.transform.forward * forwardOffset;
        }
        else
        {
            // 攻撃前はレールを追従
            FollowPlayerRail();
        }
    }

    private void FollowPlayerRail()
    {
        var playerMove = player.GetComponent<PlayerMove>(); // PlayerMoveコンポーネントを取得
        if (playerMove != null && playerMove.CurrentRail != null)
        {
            // プレイヤーが乗っているレールの位置を特定
            currentRail = playerMove.CurrentRail;

            // モブが降り立った位置の近くにあるレールを取得する
            Vector3 nearestRailPosition = GetNearestRailPosition(spawnPosition);

            // 近くのレール位置に移動
            transform.position = nearestRailPosition;
            transform.forward = currentRail.GetSampleAtDistance(playerMove._railPosition * currentRail.Length).tangent;
        }
    }

    private Vector3 GetNearestRailPosition(Vector3 spawnPos)
    {
        var playerMove = player.GetComponent<PlayerMove>();
        if (playerMove != null && playerMove.CurrentRail != null)
        {
            // スポーン位置の近くにあるレールを特定
            return currentRail.GetSampleAtDistance(playerMove._railPosition * currentRail.Length).location;
        }

        return spawnPos; // 近くのレールが見つからなければ、スポーン位置を返す
    }

    public bool IsAttacking()
    {
        return isAttacking;
    }
}

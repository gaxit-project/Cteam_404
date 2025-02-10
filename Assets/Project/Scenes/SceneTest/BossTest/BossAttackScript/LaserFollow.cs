using UnityEngine;
using SplineMesh;

public class LaserFollow : MonoBehaviour
{
    private PlayerMove player;
    private float forwardOffset;
    private float lifeTime = 3f;
    private float timer = 0f;

    public void Initialize(PlayerMove targetPlayer, float offset)
    {
        player = targetPlayer;
        forwardOffset = offset;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= lifeTime)
        {
            Destroy(gameObject);
            return;
        }

        if (player == null || player.CurrentRail == null)
        {
            Destroy(gameObject);
            return;
        }

        // プレイヤーの進行方向の少し先を取得
        Spline playerRail = player.CurrentRail;
        float targetPos = Mathf.Clamp(player._speed * Time.deltaTime / playerRail.Length + (forwardOffset / playerRail.Length), 0f, 1f);
        var splineSample = playerRail.GetSampleAtDistance(targetPos * playerRail.Length);

        // レーザーの位置を更新
        transform.position = splineSample.location;
        transform.rotation = Quaternion.LookRotation(splineSample.tangent);
    }
}

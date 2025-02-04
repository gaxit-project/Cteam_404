using UnityEngine;
using SplineMesh;

[CreateAssetMenu(fileName = "LaserAttack", menuName = "BossAttacks/Laser")]
public class LaserAttack : ScriptableObject, IBossAttack
{
    [Header("レーザーのプレハブ")]
    public GameObject laserPrefab;

    [Header("発射位置のオフセット")]
    public float forwardOffset = 3f;

    public void ExecuteAttack(GameObject boss)
    {
        PlayerMove player = GameObject.FindObjectOfType<PlayerMove>();
        if (player == null || player.CurrentRail == null)
        {
            Debug.LogWarning("プレイヤーまたはレールが見つかりません");
            return;
        }

        GameObject laser = Instantiate(laserPrefab);
        LaserFollow followScript = laser.AddComponent<LaserFollow>();

        // 追従用スクリプトにプレイヤー情報を渡す
        followScript.Initialize(player, forwardOffset);
    }
}

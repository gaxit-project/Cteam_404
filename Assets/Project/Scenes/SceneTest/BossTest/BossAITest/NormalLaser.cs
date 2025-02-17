using UnityEngine;

public class NormalLaser : MonoBehaviour
{
    public Transform centerObject; // 円の中心
    public Transform player; // プレイヤー
    public float lookAheadTime = 0.5f; // どれくらい前の位置を取得するか（秒）
    public ParticleSystem laserParticle; // レーザーのパーティクルシステム

    public void ExecuteAttack()
    {
        Debug.Log("Boss はレーザーを発射した！");

        // プレイヤーの現在の位置と速度を取得
        Vector3 playerPosition = player.position;
        Vector3 playerVelocity = player.GetComponent<Rigidbody>().velocity;

        // プレイヤーの進行方向を予測
        Vector3 predictedPosition = playerPosition + playerVelocity * lookAheadTime;

        // 円の中心から予測位置へのベクトルを計算
        Vector3 direction = (predictedPosition - centerObject.position).normalized;

        // レーザーの発射位置と方向を設定
        transform.position = centerObject.position;
        transform.forward = direction;

        // パーティクルシステムを再生
        laserParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        laserParticle.Play();

        // レーザーの発射処理をここに追加
        // 例えば、レーザーの衝突判定やダメージ処理など
    }
}

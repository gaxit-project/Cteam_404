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

        // �v���C���[�̐i�s�����̏�������擾
        Spline playerRail = player.CurrentRail;
        float targetPos = Mathf.Clamp(player._speed * Time.deltaTime / playerRail.Length + (forwardOffset / playerRail.Length), 0f, 1f);
        var splineSample = playerRail.GetSampleAtDistance(targetPos * playerRail.Length);

        // ���[�U�[�̈ʒu���X�V
        transform.position = splineSample.location;
        transform.rotation = Quaternion.LookRotation(splineSample.tangent);
    }
}

using System.Collections;
using UnityEngine;
using SplineMesh;

public class MobLaserAttack : MonoBehaviour, IBossAttack
{
    [Header("�U����������")]
    [SerializeField] private float prepareTime = 2f;

    [Header("�r�[�����ˌ�̏��Ŏ���")]
    [SerializeField] private float disappearTime = 3f;

    [Header("�o���ʒu�I�t�Z�b�g")]
    [SerializeField] private float spawnHeight = 5f; // �v���C���[�̏�ɏo�����鍂��
    [SerializeField] private float forwardOffset = 3f; // �v���C���[�̑O�ɏo�����鋗��
    [SerializeField] private float fadeInDuration = 0.2f; // �t�F�[�h�C������

    private GameObject player;
    private bool isPreparing = false;
    private bool isAttacking = false;
    private Spline currentRail; // ���݂̃��[���i�Ǐ]�p�j
    private Vector3 spawnPosition; // �X�|�[���ʒu���L�^

    public void ExecuteAttack(GameObject boss)
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        // �v���C���[�̏ォ�����O�ɃX�|�[��
        spawnPosition = player.transform.position + Vector3.up * spawnHeight + player.transform.forward * forwardOffset;
        transform.position = spawnPosition;

        StartCoroutine(AttackSequence());
    }

    private IEnumerator AttackSequence()
    {
        // �ォ�牺�Ƀt�F�[�h�C��
        yield return FadeInToPosition();

        // �v���C���[�̃��[����Ǐ]�i�ŏ��ɍ~�藧�������[�����擾�j
        yield return new WaitForSeconds(prepareTime);

        // �U�������J�n�i���[���ړ��𖳎��j
        isPreparing = true;

        // ���b��Ƀr�[������
        yield return new WaitForSeconds(prepareTime);
        FireBeam();

        // �r�[�����ˌ�t�F�[�h�A�E�g
        yield return new WaitForSeconds(disappearTime);
        yield return FadeOut();

        Destroy(gameObject);
    }

    private void FireBeam()
    {
        Debug.Log("���u���r�[���𔭎ˁI");
        // �r�[�����˃��W�b�N��ǉ�
    }

    private IEnumerator FadeInToPosition()
    {
        float elapsedTime = 0f;
        Vector3 start = transform.position;
        Vector3 target = player.transform.position + player.transform.forward * forwardOffset; // �v���C���[�̑O���ֈړ�

        // �t�F�[�h�C�����A�v���C���[������̋�����ۂ��Ȃ���ړ�
        while (elapsedTime < fadeInDuration)
        {
            // �v���C���[������̋�����ۂ�
            Vector3 direction = (target - player.transform.position).normalized;
            transform.position = Vector3.Lerp(start, target, elapsedTime / fadeInDuration);
            transform.position = player.transform.position + direction * forwardOffset;  // �v���C���[������̋���

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = player.transform.position + (target - player.transform.position).normalized * forwardOffset;
    }

    private IEnumerator FadeOut()
    {
        Debug.Log("���u���t�F�[�h�A�E�g");
        yield return new WaitForSeconds(1f);
    }

    private void Update()
    {
        if (player == null) return;

        // �U���������n�܂����烌�[���̒ǔ����~
        if (isPreparing)
        {
            // �U�������i�K�ł̓v���C���[�Ɠ������[���ɒǏ]
            transform.position = player.transform.position + player.transform.forward * forwardOffset;
        }
        else
        {
            // �U���O�̓��[����Ǐ]
            FollowPlayerRail();
        }
    }

    private void FollowPlayerRail()
    {
        var playerMove = player.GetComponent<PlayerMove>(); // PlayerMove�R���|�[�l���g���擾
        if (playerMove != null && playerMove.CurrentRail != null)
        {
            // �v���C���[������Ă��郌�[���̈ʒu�����
            currentRail = playerMove.CurrentRail;

            // ���u���~�藧�����ʒu�̋߂��ɂ��郌�[�����擾����
            Vector3 nearestRailPosition = GetNearestRailPosition(spawnPosition);

            // �߂��̃��[���ʒu�Ɉړ�
            transform.position = nearestRailPosition;
            transform.forward = currentRail.GetSampleAtDistance(playerMove._railPosition * currentRail.Length).tangent;
        }
    }

    private Vector3 GetNearestRailPosition(Vector3 spawnPos)
    {
        var playerMove = player.GetComponent<PlayerMove>();
        if (playerMove != null && playerMove.CurrentRail != null)
        {
            // �X�|�[���ʒu�̋߂��ɂ��郌�[�������
            return currentRail.GetSampleAtDistance(playerMove._railPosition * currentRail.Length).location;
        }

        return spawnPos; // �߂��̃��[����������Ȃ���΁A�X�|�[���ʒu��Ԃ�
    }

    public bool IsAttacking()
    {
        return isAttacking;
    }
}

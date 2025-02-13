using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("�^�[�Q�b�g�ݒ�")]
    public Transform player; // �v���C���[��Transform
    public Transform boss;   // �{�X��Transform

    [Header("�J�����ړ��ݒ�")]
    public float baseDistance = 5f; // �v���C���[����̊�{����
    public float minDistance = 3f;  // �ŏ�����
    public float maxDistance = 12f; // �ő勗��
    public float heightOffset = 1.5f; // �v���C���[�̓���ʒu
    public float lateralOffset = 1.0f; // �J�����̉��ʒu�␳

    [Header("�΂˃J�����ݒ�")]
    public float springStrength = 10f; // �΂˂̋���
    public float damping = 5f;         // �����i�_���s���O�j

    [Header("FOV�ݒ�")]
    public float minFOV = 40f;  // �ŏ�FOV
    public float maxFOV = 80f;  // �ő�FOV
    public float fovPadding = 2f; // �]��
    public float fovSmoothing = 5f; // FOV�̃X���[�W���O

    private Vector3 velocity; // �J�����ړ��̑��x
    private Camera cam;       // �J�����R���|�[�l���g

    private void Start()
    {
        cam = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        if (player == null || boss == null) return;

        // 1. �v���C���[�ƃ{�X�̒��ԓ_���擾
        Vector3 midpoint = (player.position + boss.position) / 2;

        // 2. �J�����̎��_�ʒu���v�Z�i�v���C���[�̌���ɔz�u�j
        Vector3 playerToBoss = (boss.position - player.position).normalized; // �v���C���[���{�X�̕���
        Vector3 targetCameraPosition = player.position
                                      - playerToBoss * baseDistance // �v���C���[�̌��ɔz�u
                                      + Vector3.up * heightOffset   // ������ɃI�t�Z�b�g
                                      + Vector3.right * lateralOffset; // �������̕␳

        // 3. �v���C���[�ƃ{�X�̋����ɉ����ăJ�����̋����𒲐�
        float dynamicDistance = Mathf.Clamp(Vector3.Distance(player.position, boss.position), minDistance, maxDistance);
        targetCameraPosition = player.position - playerToBoss * dynamicDistance + Vector3.up * heightOffset;

        // 4. �΂˃J�����̈ړ��v�Z
        Vector3 springForce = (targetCameraPosition - transform.position) * springStrength;
        Vector3 dampingForce = -velocity * damping;
        velocity += (springForce + dampingForce) * Time.deltaTime;
        transform.position += velocity * Time.deltaTime;

        // 5. �{�X�����߂�悤�ɃJ��������]
        transform.LookAt(boss.position);

        // 6. FOV�𒲐����ăv���C���[�ƃ{�X�����߂�
        AdjustFOV();
    }

    private void AdjustFOV()
    {
        float targetDistance = Vector3.Distance(player.position, boss.position);
        float targetFOV = Mathf.Clamp(targetDistance + fovPadding, minFOV, maxFOV);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * fovSmoothing);
    }
}

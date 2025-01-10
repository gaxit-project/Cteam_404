using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float Speed;

    private Vector3 MoveValue;


    // ���߂������������̃x�N�g��
    private Vector3 _direction = Vector3.forward;
    // 1�t���[���O�̈ʒu
    private Vector3 _prevPosition;

    Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // �����ʒu��ێ�
        _prevPosition = transform.position;

        // ���������̃x�N�g���𐳋K��
        _direction.Normalize();
    }

    void Update()
    {
        // deltaTime��0�̏ꍇ�͉������Ȃ�
        if (Mathf.Approximately(Time.deltaTime, 0))
            return;

        // ���݈ʒu�擾
        var position = transform.position;

        // ���ݑ��x�v�Z
        var velocity = (position - _prevPosition) / Time.deltaTime;

        // ���������̃x�N�g���Ƒ��x�̓��ς����߂�
        var directionalVelocity = Vector3.Dot(velocity, _direction);

        // ���ʕ\��
        print($"directionalVelocity = {directionalVelocity}");

        // �O�t���[���ʒu���X�V
        _prevPosition = position;
    }

    void FixedUpdate()
    {


        MoveValue = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
        MoveValue.Normalize();



        rb.AddForce(MoveValue * Speed);
    }
}

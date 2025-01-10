using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float Speed;

    private Vector3 MoveValue;


    // 求めたい方向成分のベクトル
    private Vector3 _direction = Vector3.forward;
    // 1フレーム前の位置
    private Vector3 _prevPosition;

    Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // 初期位置を保持
        _prevPosition = transform.position;

        // 方向成分のベクトルを正規化
        _direction.Normalize();
    }

    void Update()
    {
        // deltaTimeが0の場合は何もしない
        if (Mathf.Approximately(Time.deltaTime, 0))
            return;

        // 現在位置取得
        var position = transform.position;

        // 現在速度計算
        var velocity = (position - _prevPosition) / Time.deltaTime;

        // 方向成分のベクトルと速度の内積を求める
        var directionalVelocity = Vector3.Dot(velocity, _direction);

        // 結果表示
        print($"directionalVelocity = {directionalVelocity}");

        // 前フレーム位置を更新
        _prevPosition = position;
    }

    void FixedUpdate()
    {


        MoveValue = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
        MoveValue.Normalize();



        rb.AddForce(MoveValue * Speed);
    }
}

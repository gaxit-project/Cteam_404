using UnityEngine;


public class Player : MonoBehaviour
{
    public float speed;

    public float MoveHorizontalValue;
    public float MoveVerticalValue;
    public Vector3 MoveValue;

    private void Start()
    {

    }


}





public class PlayerController : Player
{

    Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        
    }

    void FixedUpdate()
    {


        MoveValue = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
        MoveValue.Normalize();



        rb.AddForce(MoveValue * speed);
    }
}

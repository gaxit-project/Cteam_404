using UnityEngine;

public class StopTime : MonoBehaviour
{
    private bool isMove;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isMove = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isMove)
            {
                isMove = false;
                Time.timeScale = 0;
            }
            else
            {
                isMove = true;
                Time.timeScale = 1;
            }

        }
    }
}

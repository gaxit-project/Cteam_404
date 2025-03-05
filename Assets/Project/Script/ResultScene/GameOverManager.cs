using UnityEngine;

public class GameOverManager : MonoBehaviour
{

    void Start()
    {
        AudioManager.GetInstance().PlayBGM(4);
    }

}

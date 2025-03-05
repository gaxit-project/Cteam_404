using UnityEngine;

public class MainSceneManager : MonoBehaviour
{
    void Start()
    {
        AudioManager.GetInstance().PlayBGM(2);
    }

}

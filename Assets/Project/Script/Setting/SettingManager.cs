using UnityEngine;

public class SettingManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AudioManager.GetInstance().PlayBGM(1);
    }
}

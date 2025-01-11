using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    [SerializeField] private string _sceneName;
    public void SceneChangeToMainScene() // start�{�^���������ƃ��C���V�[���ɑJ��
    {
        SceneManager.LoadScene(_sceneName);
    }
    public void ApplicationEnd() // quit�{�^���������ƃQ�[�����I��
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // �Q�[���I��
        #else
            Application.Quit(); // �Q�[���I��
        #endif
    }
}

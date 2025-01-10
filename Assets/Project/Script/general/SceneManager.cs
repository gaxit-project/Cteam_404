using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [SerializeField] private string _sceneName;

    public void ChangeScene() // �V�[���`�F���W
    {
        SceneManager.LoadScene( _sceneName );
    }
    public void EndScene() // �Q�[���I��
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // �Q�[���I��
        #else
            Application.Quit(); // �Q�[���I��
        #endif
    }
}

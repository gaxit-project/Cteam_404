using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class SceneChangeManager : MonoBehaviour
{
    public static SceneChangeManager Instance = null;
    #region �V���O���g��

    public static SceneChangeManager GetInstance()
    {
        if(Instance == null)
        {
            Instance = FindObjectOfType<SceneChangeManager>();
        }
        return Instance;
    }
    private void Awake()
    {
        if(this != GetInstance())
        {
            Destroy(this.gameObject);
            return;
        }
        DontDestroyOnLoad(this.gameObject);
    }
    #endregion
    
    [SerializeField] private string _sceneName;
    public void SceneChange() // start�{�^���������ƃ��C���V�[���ɑJ��
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

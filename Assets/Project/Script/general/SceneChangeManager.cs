using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class SceneChangeManager : MonoBehaviour
{
    public static SceneChangeManager Instance {  get; private set; }


    #region シングルトン

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


    public void SceneChange(string sceneName) // startボタンを押すとメインシーンに遷移
    {
        SceneManager.LoadScene(sceneName);
    }
    public void ApplicationEnd() // quitボタンを押すとゲームを終了
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // ゲーム終了
        #else
            Application.Quit(); // ゲーム終了
        #endif
    }


    
}

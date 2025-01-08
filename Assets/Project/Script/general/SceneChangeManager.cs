using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeManager : MonoBehaviour
{
    [SerializeField] private string _sceneName;
    public void SceneChange() // startボタンを押すとメインシーンに遷移
    {
        SceneManager.LoadScene(_sceneName);
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

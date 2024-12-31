using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [SerializeField] private string _sceneName;

    public void ChangeScene() // シーンチェンジ
    {
        SceneManager.LoadScene( _sceneName );
    }
    public void EndScene() // ゲーム終了
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // ゲーム終了
        #else
            Application.Quit(); // ゲーム終了
        #endif
    }
}

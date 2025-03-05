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

    #region SEがなっているか確認

    public void SceneChangeWithSE(string sceneName)
    {
        AudioManager audioManager = AudioManager.GetInstance(); // AudioManagerのインスタンスを取得

        if (audioManager != null && audioManager._audioSourceSE != null)
        {
            StartCoroutine(WaitForSoundEnd(audioManager._audioSourceSE, sceneName)); //コルーチンスタート
        }
        else
        {
            Debug.LogError("AudioManagerまたはAudioSourceが見つかりません");
        }
    }

    IEnumerator WaitForSoundEnd(AudioSource audioSource, string sceneName)
    {
        while (audioSource.isPlaying)
        {
            yield return null;
        }

        SceneChange(sceneName);
    }

    #endregion

    #region クリア判定
    private void Update()
    {
        int BuildIndex = SceneManager.GetActiveScene().buildIndex;
        

        if(BuildIndex == 2)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                GameOver();
            }
            else if (Input.GetKeyDown(KeyCode.RightShift))
            {
                GameClear();
            }
        }
    }

    public void GameOver()
    {
        Debug.Log("Game Over");
        SceneManager.LoadScene("GameOver");
    }

    public void GameClear()
    {
        Debug.Log("Game Clear");
        SceneManager.LoadScene("GameClear");
    }

    #endregion
    public void SceneChange(string sceneName) // startボタンを押すとメインシーンに遷移
    {
        SceneManager.LoadSceneAsync(sceneName);
        AudioManager.GetInstance().StopBGM();
        Time.timeScale = 1.0f;
    }




    
}

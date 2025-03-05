using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;
    
    public GameObject Canvas;

    private int BuildIndex;

    private bool IsPaused = false;


    #region シングルトン

    public static PauseManager GetInstance()
    {
        if (Instance == null)
        {
            Instance = FindObjectOfType<PauseManager>();
        }
        return Instance;
    }
    private void Awake()
    {
        BuildIndex = SceneManager.GetActiveScene().buildIndex;
        
        if (this != GetInstance())
        {
            Destroy(this.gameObject);
            return;
        }
        DontDestroyOnLoad(this.gameObject);

    }
    #endregion

    #region Sceneを移動したときの処理
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        BuildIndex = scene.buildIndex; // ビルド番号を取得
        
        if (BuildIndex == 2)
        {
            if (Canvas == null) // Canvasがnullなら
            {
                Canvas = GameObject.Find("Canvas");  //Canvasを取得
                Canvas.SetActive(false);  //取得してから非アクティブに変更
            }
        }
    }

    #endregion

    public void OnPausu(InputAction.CallbackContext context)
    {

        // Performedフェーズの判定を行う
        if (context.phase == InputActionPhase.Performed)
        {

            IsPaused = true;
        }
    }


    /// <summary>
    /// escキーが押されたらCanvasを実体化
    /// </summary>
    private void Update()
    {
        if (BuildIndex == 2) //MainScene内なら
        {
            if (Canvas == null)
            {
                Canvas = GameObject.Find("Canvas");
            }

            if (IsPaused)
            {
                IsPaused = false;
                Time.timeScale = 0f;
                if (Canvas != null)
                {
                    Canvas.SetActive(true);
                    AudioManager.GetInstance().InitializeSliders();
                }
                else
                {
                    Debug.Log("Error!!");
                }
            }
        }
    } 

    /// <summary>
    /// Canvasを見つけたいときのメソッド
    /// AudioManagerで使用
    /// </summary>
    /// <returns></returns>
    public GameObject GetCanvas()
    {
        return Canvas;
    }
}

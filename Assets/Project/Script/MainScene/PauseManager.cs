using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    private static PauseManager Instance;
    public static PauseManager GetInstance()
    {
        return Instance;
    }

    public GameObject Canvas;

    private PlayerInput playerInput;

    private int BuildIndex;

    private bool IsPaused = false;

    void Awake()
    {
        Instance = this;

        BuildIndex = SceneManager.GetActiveScene().buildIndex;
    }


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
            Canvas = GameObject.Find("Canvs");
            if (Canvas == null) // Canvasがnullなら
            {
                Canvas = GameObject.Find("Canvas");  //Canvasを取得
                Canvas.SetActive(false);  //取得してから非アクティブに変更
            }
            else
            {
                Canvas.SetActive(false);
            }

        }
    }

    public void RegisterPlayerInput(PlayerInput newPlayerInput)
    {
        if (playerInput == null)
        {
            //古いPlayerInputのリスナーを解除
            playerInput.actions.FindActionMap("Player").FindAction("Pause").performed -= OnPause;
        }

        playerInput = newPlayerInput;
        if (playerInput != null)
        {
            InputAction pauseAction = playerInput.actions.FindActionMap("Player").FindAction("Pause");
            if (pauseAction != null)
            {
                pauseAction.performed += OnPause;
            }
        }
    }

    #endregion

    public void OnPause(InputAction.CallbackContext context)
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


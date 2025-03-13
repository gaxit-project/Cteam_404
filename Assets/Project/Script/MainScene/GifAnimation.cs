using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GifAnimation : MonoBehaviour
{
    [Header("Gif画像")]
    [SerializeField] private Texture2D[] _GifImage;
    [Header("ループ")]
    public bool Loop;

    private RawImage _image;
    private Coroutine _animationCoroutine;
    private bool _isPlaying = false;

    [SerializeField] private float _frameRate = 0.1f; // フレーム間の時間

    void Awake()
    {
        _image = GetComponent<RawImage>();
        if (_GifImage.Length > 0)
        {
            _image.texture = _GifImage[0]; // 初期画像を設定
        }
    }

    public void Play()
    {
        if (_isPlaying || _GifImage.Length == 0) return;

        _isPlaying = true;
        _animationCoroutine = StartCoroutine(PlayGif());
    }

    public void Stop()
    {
        if (_animationCoroutine != null)
        {
            StopCoroutine(_animationCoroutine);
        }
        _isPlaying = false;

        if (_GifImage.Length > 0)
        {
            _image.texture = _GifImage[0]; // 初期フレームに戻す
        }
    }

    private IEnumerator PlayGif()
    {
        do
        {
            for (int i = 0; i < _GifImage.Length; i++)
            {
                _image.texture = _GifImage[i];
                yield return new WaitForSeconds(_frameRate);
            }
        } while (Loop && _isPlaying);

        _isPlaying = false;
    }
}

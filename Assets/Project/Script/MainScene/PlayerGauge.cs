using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public class PlayerGauge : MonoBehaviour
{


    [Header("Holdゲージ")]
    [SerializeField]
    private Image _holdImage;

    [Header("Energyゲージ")]
    [SerializeField]
    private Image _energyImage;

    [Header("ボタン画像")]
    [SerializeField]
    private Image _buttonImage;

    [Header("テキスト")]
    [SerializeField]
    private TextMeshProUGUI _text;

    [Header("ULT使用可能テキスト")]
    [SerializeField]
    private string _text1;

    [Header("ULT使用不可テキスト")]
    [SerializeField]
    private string _text2;

    [Header("ULT使用可能カラー")]
    [SerializeField]
    private Color _color1;

    [Header("ULT使用不可カラー")]
    [SerializeField]
    private Color _color2;

    [Header("Gifアニメーション")]
    [SerializeField]
    private GameObject _gifAni;
    private GifAnimation _gif;

    private bool _isGifActive = false;
    private bool _isUltAvailable = false;

    void Start()
    {
        _gif = _gifAni.GetComponent<GifAnimation>();
        _gifAni.SetActive(false);
        _buttonImage.color = _color2;
        _text.color = _color2;
        _text.text = _text2;
    }

    void Update()
    {
        
    }
    private void LateUpdate()
    {
        _holdImage.fillAmount = Player.progress;
        _energyImage.fillAmount = Player.UltGauge;









        // ULTゲージが最大になったら GIF を一度だけ起動
        if (_energyImage.fillAmount >= 1f)
        {
            if (!_isGifActive)
            {
                _gifAni.SetActive(true);
                _gif.Play();
                _isGifActive = true;
            }
            _buttonImage.color = _color1;
            _text.color = _color1;
        }
        else
        {
            if (_isGifActive)
            {
                _gifAni.SetActive(false);
                _gif.Stop();
                _isGifActive = false;
            }
            _buttonImage.color = _color2;
            _text.color = _color2;
            _text.text = _text2;
        }

        // Holdゲージが最大になったら ULT使用可能テキストを一度だけ変更
        bool canUseUlt = _holdImage.fillAmount >= 1f;
        if (canUseUlt != _isUltAvailable)
        {
            _text.text = canUseUlt ? _text1 : _text2;
            _isUltAvailable = canUseUlt;
        }



        /*
        if (_energyImage.fillAmount >= 1f)
        {
            _gifAni.SetActive(true);
            _buttonImage.color = _color1;
            _text.color = _color1;
        }
        else if (_energyImage.fillAmount <= 0f)
        {
            _gifAni.SetActive(false);
            _buttonImage.color = _color2;
            _text.color = _color2;
            _text.text = _text2;
        }
        else
        {
            _buttonImage.color = _color2;
            _text.color = _color2;
            _text.text = _text2;
        }

        if (_holdImage.fillAmount >= 1f)
        {
            _text.text = _text1;
        }
        else
        {
            _text.text = _text2;
        }
        */
    }
}

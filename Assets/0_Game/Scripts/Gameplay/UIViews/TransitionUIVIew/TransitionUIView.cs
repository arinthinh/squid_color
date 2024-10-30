using Cysharp.Threading.Tasks;
using DG.Tweening;
using JSAM;
using Toolkit.UI;
using UnityEngine;
using UnityEngine.UI;

public class TransitionUIView : UIView
{
    [SerializeField] private Image _swipeImage;

    private Tween _animationTween;
    private Material _swipeMat;


    private void Start()
    {
        _swipeMat = _swipeImage.material;
    }

    public new async UniTask Show()
    {
        base.Show();
        AudioManager.PlaySound(ESound.TransitionSoundSO);
        _animationTween?.Kill();
        _animationTween = DOVirtual.Float(
                1,
                0,
                0.5f,
                value => _swipeMat.SetFloat("_Circle_Size", value))
            .SetEase(Ease.Linear);
        await _animationTween;
    }

    public new async UniTask Hide()
    {
        _animationTween?.Kill();
        _animationTween = DOVirtual.Float(
                0,
                1,
                0.8f,
                value => _swipeMat.SetFloat("_Circle_Size", value))
            .SetEase(Ease.Linear);
        await _animationTween;
        base.Hide();
    }
}
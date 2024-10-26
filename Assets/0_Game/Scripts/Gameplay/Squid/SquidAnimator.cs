using System;
using DG.Tweening;
using UnityEngine;

public class SquidAnimator : SquidController
{
    [SerializeField] private float _rotateValue;

    [SerializeField] private Transform _rotateTransform;
    [SerializeField] private Transform _scaleTransform;

    public void PlayAnimation(EAnimation anim)
    {
        switch (anim)
        {
            case EAnimation.None:
                break;
            case EAnimation.Idle:
                break;
            case EAnimation.MoveLeft:
            {
                _rotateTransform.DOKill();
                _rotateTransform.localEulerAngles = Vector3.zero;
                var animDuration = _config.MoveDuration / 2f;
                _rotateTransform.DORotate(new Vector3(0, 0, 20), animDuration)
                    .OnComplete(() => _rotateTransform.DORotate(Vector3.zero, animDuration));
                break;
            }
                
            case EAnimation.MoveRight:
            {
                _rotateTransform.DOKill();
                _rotateTransform.localEulerAngles = Vector3.zero;
                var animDuration = _config.MoveDuration / 2f;
                _rotateTransform.DORotate(new Vector3(0, 0, -20), animDuration)
                    .OnComplete(() => _rotateTransform.DORotate(Vector3.zero, animDuration));
                break;
            }
                
            case EAnimation.Shoot:
                _scaleTransform?.DOKill();
                _scaleTransform.localScale = Vector3.one;
                _scaleTransform.DOPunchScale(Vector3.one * 0.2f, 0.2f);
                break;
        }
    }

    public enum EAnimation
    {
        None,
        Idle,
        MoveLeft,
        MoveRight,
        Shoot,
    }
}
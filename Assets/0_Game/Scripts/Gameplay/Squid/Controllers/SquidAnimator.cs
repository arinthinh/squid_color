using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class SquidAnimator : SquidController
{
    [SerializeField] private float _rotateValue;

    [SerializeField] private SpriteRenderer _stunEyes;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Transform _rotateTransform;
    [SerializeField] private Transform _scaleTransform;

    [Button]
    public void PlayAnimation(EAnimation anim)
    {
        switch (anim)
        {
            case EAnimation.None:
                break;
            case EAnimation.Idle:
            {
                _rotateTransform.DOKill();
                _rotateTransform.localEulerAngles = Vector3.zero;
                _stunEyes.gameObject.SetActive(false);
                
                _spriteRenderer.DOKill();
                _spriteRenderer.color = Color.white;
                break;
            }
            case EAnimation.MoveLeft:
            {
                _rotateTransform.DOKill();
                _rotateTransform.localEulerAngles = Vector3.zero;
                var animDuration = _config.MoveDuration / 2f;
                _rotateTransform.DORotate(new Vector3(0, 0, _rotateValue), animDuration)
                    .OnComplete(() => _rotateTransform.DORotate(Vector3.zero, animDuration));
                break;
            }

            case EAnimation.MoveRight:
            {
                _rotateTransform.DOKill();
                _rotateTransform.localEulerAngles = Vector3.zero;
                var animDuration = _config.MoveDuration / 2f;
                _rotateTransform.DORotate(new Vector3(0, 0, -_rotateValue), animDuration)
                    .OnComplete(() => _rotateTransform.DORotate(Vector3.zero, animDuration));
                break;
            }

            case EAnimation.Shoot:
                _scaleTransform?.DOKill();
                _scaleTransform.localScale = Vector3.one;
                _scaleTransform.DOPunchScale(Vector3.one * 0.2f, 0.2f);
                break;

            case EAnimation.Stun:
            {
                _rotateTransform.DOKill();
                var haftRotateValue = _rotateValue / 2f ;
                _rotateTransform.localEulerAngles = Vector3.forward * -haftRotateValue;
                _stunEyes.gameObject.SetActive(true);
                _rotateTransform.DORotate(Vector3.forward * haftRotateValue, 0.25f)
                    .SetEase(Ease.Linear)
                    .SetLoops(-1, LoopType.Yoyo);
                break;
            }

            case EAnimation.Invisible:
            {
                _spriteRenderer.DOFade(0, 0.15f).SetLoops(-1, LoopType.Yoyo);
                break;
            }
        }
    }

    public enum EAnimation
    {
        None,
        Idle,
        MoveLeft,
        MoveRight,
        Shoot,
        Stun,
        Invisible
    }
}
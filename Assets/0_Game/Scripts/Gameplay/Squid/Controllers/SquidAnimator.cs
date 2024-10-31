using System;
using System.Collections.Generic;
using DG.Tweening;
using Redcode.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

public class SquidAnimator : SquidController
{
    [SerializeField] private float _rotateValue;

    [Header("SWITCH COLOR")]
    [SerializeField] private SquidSpritesConfigSO _spritesConfig;
    [SerializeField] private SpriteRenderer _currentSpriteRenderer;
    [SerializeField] private SpriteRenderer _newSpriteRenderer;
    [SerializeField] private float _basePosY;
    [SerializeField] private float _moveDownPosY;

    [Header("STUN")]
    [SerializeField] private List<SpriteRenderer> _stunEyes;

    [Header("MOVE AND ATTACK")]
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
                _stunEyes.ForEach(e => e.gameObject.SetActive(false));


                _currentSpriteRenderer.DOKill();
                _currentSpriteRenderer.color = Color.white;
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
                var haftRotateValue = _rotateValue / 2f;
                _rotateTransform.localEulerAngles = Vector3.forward * -haftRotateValue;
                _stunEyes.ForEach(e => e.gameObject.SetActive(true));
                _rotateTransform.DORotate(Vector3.forward * haftRotateValue, 0.25f)
                    .SetEase(Ease.Linear)
                    .SetLoops(-1, LoopType.Yoyo);
                break;
            }

            case EAnimation.Invisible:
            {
                _currentSpriteRenderer.DOFade(0.5f, 0.15f).SetLoops(-1, LoopType.Yoyo);
                break;
            }
        }
    }

    public void PlaySwitchColorAnimation(EColor newColor, bool isImmediate = false)
    {
        var newColorSprite = _spritesConfig.GetSprite(newColor);

        if (isImmediate)
        {
            _currentSpriteRenderer.sprite = newColorSprite;
            _currentSpriteRenderer.transform.SetLocalPositionY(_basePosY);
            _newSpriteRenderer.transform.SetLocalPositionY(_moveDownPosY);
            return;
        }

        _newSpriteRenderer.sprite = newColorSprite;
        _currentSpriteRenderer.transform.DOLocalMoveY(_moveDownPosY, 0.25f);
        _newSpriteRenderer.transform.DOLocalMoveY(_basePosY, 0.25f);
        (_currentSpriteRenderer, _newSpriteRenderer) = (_newSpriteRenderer, _currentSpriteRenderer);
    }

    public enum EAnimation
    {
        None,
        Idle,
        MoveLeft,
        MoveRight,
        Shoot,
        Stun,
        Invisible,
        SwitchColor
    }
}
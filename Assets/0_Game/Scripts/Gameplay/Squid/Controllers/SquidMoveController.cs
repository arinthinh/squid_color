using System;
using DG.Tweening;
using JSAM;
using Redcode.Extensions;
using UnityEngine;
using UnityEngine.InputSystem;

public class SquidMoveController : SquidController
{
    [Header("RENDERER")]
    [SerializeField] private Transform _moveTransform;

    [Header("ANIMATOR")]
    [SerializeField] private SquidAnimator _animator;
    [SerializeField] private SquidStunController _stunController;

    [Header("INPUT ACTIONS")]
    [SerializeField] private InputActionReference _moveLeftAction;
    [SerializeField] private InputActionReference _moveRightAction;

    private void OnEnable()
    {
        _moveLeftAction.action.started += OnInputMoveLeft;
        _moveRightAction.action.started += OnInputMoveRight;
    }

    private void OnDisable()
    {
        _moveLeftAction.action.started -= OnInputMoveLeft;
        _moveRightAction.action.started -= OnInputMoveRight;
    }

    public override void OnStartPlay()
    {
        transform.SetPositionX(0);
        SetEnableMove(true);
    }

    public override void OnStopPlay()
    {
        SetEnableMove(false);
    }

    public void SetEnableMove(bool canMove)
    {
        if (canMove)
        {
            _moveLeftAction.action.Enable();
            _moveRightAction.action.Enable();
        }
        else
        {
            _moveLeftAction.action.Disable();
            _moveRightAction.action.Disable();
        }
    }

    public void Move(MoveDirection direction)
    {
        if (_stunController.IsStun) return;
        
        var nextPosition = _data.Position + (direction == MoveDirection.Left ? -1 : 1);
        if (Mathf.Abs(nextPosition) > _config.MaxMoveIndex) return;
        _data.Position = nextPosition;

        var targetPosition = new Vector3(_data.Position * _config.MoveDistance, _moveTransform.position.y, _moveTransform.position.z);

        // Move to the target position with snapping
        _moveTransform.DOKill();
        _moveTransform.DOMove(targetPosition, _config.MoveDuration).SetEase(Ease.OutExpo);
        
        // Play sound
        AudioManager.PlaySound(ESound.MoveSoundSO);

        // Play animation
        _animator.PlayAnimation(direction == MoveDirection.Left ? SquidAnimator.EAnimation.MoveLeft : SquidAnimator.EAnimation.MoveRight);
    }

    private void OnInputMoveLeft(InputAction.CallbackContext context) => Move(MoveDirection.Left);
    private void OnInputMoveRight(InputAction.CallbackContext context) => Move(MoveDirection.Right);


    public enum MoveDirection
    {
        Left,
        Right
    }
}
using System;
using DG.Tweening;
using UnityEngine;

public class SquidStunController : SquidController
{
    [SerializeField] private SquidMoveController _moveController;
    [SerializeField] private SquidAnimator _animator;
    [SerializeField] private SquidAttackController _attackController;

    private bool _isStun = false;
    private bool _isInvisible = false;
    
    private Tween _stopStunTween;
    private Tween _stopInvisibleTween;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("EnemyProjectile"))
        {
            Stun(_config.StunTime);
        }
    }

    private void Stun(float stunTime)
    {
        if(_isInvisible) return;
        if(_isStun) return;
        
        _isStun = true;

        _moveController.SetEnableMove(false);
        _attackController.SetEnableAttack(false);
        _animator.PlayAnimation(SquidAnimator.EAnimation.Stun);
        
        _stopStunTween?.Kill();
        _stopStunTween = DOVirtual.DelayedCall(stunTime, StopStun);
    }

    private void StopStun()
    {
        _stopStunTween?.Kill();
        _isStun = false;
        _moveController.SetEnableMove(true);
        _attackController.SetEnableAttack(true);
        _animator.PlayAnimation(SquidAnimator.EAnimation.Idle);
        BeInvisible();
    }

    private void BeInvisible()
    {
        _isInvisible = true;
        _stopInvisibleTween?.Kill();
        _animator.PlayAnimation(SquidAnimator.EAnimation.Invisible);
        _stopInvisibleTween = DOVirtual.DelayedCall(_config.InvisibleTime, StopInvisible);
    }
    
    private void StopInvisible()
    {
        _animator.PlayAnimation(SquidAnimator.EAnimation.Idle);
        _stopInvisibleTween?.Kill();
        _isInvisible = false;
    }
}
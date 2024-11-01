﻿using System;
using DG.Tweening;
using JSAM;
using UnityEngine;

public class SquidStunController : SquidController
{
    [SerializeField] private SquidAnimator _animator;

    private bool _isStun = false;
    private bool _isInvisible = false;

    private Tween _stopStunTween;
    private Tween _stopInvisibleTween;

    public bool IsStun => _isStun;
    public bool IsInvisible => _isInvisible;

    public override void OnStopPlay()
    {
        base.OnStopPlay();
        _isStun = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("EnemyProjectile"))
        {
            Stun(_config.StunTime);
        }
    }

    private void Stun(float stunTime)
    {
        if (_isInvisible) return;
        if (_isStun) return;

        AudioManager.PlaySound(ESound.HitSoundSO);
        _isStun = true;

        _animator.PlayAnimation(SquidAnimator.EAnimation.Stun);
        _stopStunTween?.Kill();
        _stopStunTween = DOVirtual.DelayedCall(stunTime, StopStun);
    }

    private void StopStun()
    {
        _stopStunTween?.Kill();
        _isStun = false;

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
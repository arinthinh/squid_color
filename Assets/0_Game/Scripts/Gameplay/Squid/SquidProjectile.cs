using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Pool;

public class SquidProjectile : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _colorRenderer;
    [SerializeField] private ColorDefineSO _colorDefine;
    
    private EColor _color;
    private IObjectPool<SquidProjectile> _pool;

    public EColor Color => _color;

    public void OnSpawn(IObjectPool<SquidProjectile> pool)
    {
        _pool = pool;
    }

    public void SetInfo(EColor color, Vector2 startPosition)
    {
        _color = color;
        transform.position = startPosition;
        _colorRenderer.color = _colorDefine.GetColor(color).Color;

    }

    public void Fly()
    {
        transform.DOMoveY(transform.position.y + 25, 0.75f)
            .SetEase(Ease.Linear)
            .OnComplete(Disappear);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
    }

    private void Disappear()
    {
        transform.DOKill();
        _pool.Release(this);
    }
}
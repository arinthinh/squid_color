using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Pool;

public class SquidProjectile : Projectile
{
    [SerializeField] private SpriteRenderer _colorRenderer;
    [SerializeField] private ColorDefineSO _colorDefine;
    
    private EColor _color;
    public EColor Color => _color;

    public void SetInfo(EColor color, Vector2 startPosition)
    {
        _color = color;
        transform.position = startPosition;
        _colorRenderer.color = _colorDefine.GetColor(color).Color;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Disappear();
        }
    }
}
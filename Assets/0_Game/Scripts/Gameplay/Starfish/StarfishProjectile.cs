using UnityEngine;

public class StarfishProjectile : Projectile
{
    [SerializeField] private ColorDefineSO _colorDefine;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    public void SetColor(EColor colorName)
    {
        var colorData = _colorDefine.GetColor(colorName);
        _spriteRenderer.color = colorData.Color;
    }
}
using UnityEngine;

public class SquidAvatarController : SquidController
{
    [SerializeField] private SquidSpritesConfigSO _sprites;
    [SerializeField] private SpriteRenderer _avatarRenderer;

    public void ChangeColor(EColor color)
    {
        _avatarRenderer.sprite = _sprites.GetSprite(color);
    }
}
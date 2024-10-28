using System;
using System.Linq;
using UnityEngine;

public class SquidSpritesConfigSO : ScriptableObject
{
    public SpriteConfig[] Sprites;
    
    public Sprite GetSprite(EColor color)
    {
        return Sprites.FirstOrDefault(s => s.Color == color)?.Sprite ?? Sprites[0].Sprite;
    }

    [Serializable]
    public class SpriteConfig
    {
        public EColor Color;
        public Sprite Sprite;
    }
}

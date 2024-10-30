using System;
using System.Linq;
using UnityEngine;

public class StarfishSpritesConfigSO : ScriptableObject
{
    public SpriteConfig[] Sprites;
    
    public Sprite GetSprite(EColor color, EStarfishState state)
    {
        return Sprites.FirstOrDefault(s => s.Color == color && s.State == state)?.Sprite ?? Sprites[0].Sprite;
    }

    [Serializable]
    public class SpriteConfig
    {
        public EStarfishState State;
        public EColor Color;
        public Sprite Sprite;
    }
}
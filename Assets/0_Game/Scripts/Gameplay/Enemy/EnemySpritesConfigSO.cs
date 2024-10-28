using System;
using System.Linq;
using UnityEngine;

public class EnemySpritesConfigSO : ScriptableObject
{
    public SpriteConfig[] Sprites;
    
    public Sprite GetSprite(EColor color, EEnemyState state)
    {
        return Sprites.FirstOrDefault(s => s.Color == color && s.State == state)?.Sprite ?? Sprites[0].Sprite;
    }

    [Serializable]
    public class SpriteConfig
    {
        public EEnemyState State;
        public EColor Color;
        public Sprite Sprite;
    }
}
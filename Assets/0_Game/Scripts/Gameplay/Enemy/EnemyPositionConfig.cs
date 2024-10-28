using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class EnemyPositionConfig
{
    public float Spacing;
    public int XLength;
    public int YLength;
    public List<PositionConfig> Positions;

    [Button]
    public void GenerateData(Vector3 startPosition)
    {
        Positions = new();

        for (var x = 0; x < XLength; x++)
        {
            for (var y = 0; y < YLength; y++)
            {
                Positions.Add(new(new Vector2(x, y), new Vector2(startPosition.x + x * Spacing, startPosition.y + y * Spacing)));
            }
        }
    }

    public Vector2 GetEnvironmentPosition(Vector2 index)
    {
        var inPosition = Positions.FirstOrDefault(p => p.Index == index);
        return inPosition?.Position ?? Vector2.zero;
    }
}

[Serializable]
public class PositionConfig
{
    public PositionConfig(Vector2 index, Vector2 position)
    {
        Index = index;
        Position = position;
    }

    public Vector2 Index;
    public Vector2 Position;
}
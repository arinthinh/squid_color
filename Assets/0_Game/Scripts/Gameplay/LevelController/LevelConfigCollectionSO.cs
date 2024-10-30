using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelConfigCollectionSO : ConfigCollection
{
    public List<LevelConfig> Configs;

    public LevelConfig GetLevelConfig(int id)
    {
        return Configs.First(c => c.Index == id);
    }
}

[Serializable]
public class LevelConfig
{
    public int Index;
    public int LevelTime;
    public List<TargetConfig> Targets;
    public List<EnemyWaveConfig> EnemyWaves;

    [Serializable]
    public class TargetConfig
    {
        public EColor Color;
        public int Targets;
    }
}

/// <summary>
/// This SO contains data of enemies behaviour.
/// </summary>
[Serializable]
public class EnemyWaveConfig
{
    public float Interval = 1f;
    public List<EnemyBehaviourConfig> EnemyBehaviours;
}

[Serializable]
public class EnemyBehaviourConfig
{
    /// <summary>
    /// Matrix position.
    /// </summary>
    public EColor Color;
    public Vector2 TargetPosition;
    public EDirection MoveInDirection;
    public EDirection MoveOutDirection;
    public float MoveDuration;
    public float StayInterval;
    public bool IsAttack;
}
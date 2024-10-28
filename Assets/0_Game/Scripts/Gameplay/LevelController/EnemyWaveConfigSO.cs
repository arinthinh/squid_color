using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWaveConfigSO : ScriptableObject
{
    public float Interval = 1f;
    public List<EnemyBehaviourConfig> EnemyBehaviours;
}

[Serializable]
public class EnemyBehaviourConfig
{
    public Vector2 TargetPosition;
    public EDirection MoveInDirection;
    public float MoveDuration;
    public float StayInterval;
    /// <summary>
    /// When enemy is in stay state, it will attack following this pattern.
    /// For example, if the stay time is 5s, and the pattern is [false,false,false,true, true]
    /// it means the enemy will attack at 3s and 4s (from the start of staying)
    /// </summary>
    public List<bool> AttackPattern;
}
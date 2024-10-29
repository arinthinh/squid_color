﻿using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This SO contains data of enemies behaviour.
/// </summary>
public class EnemyWaveConfigSO : ScriptableObject
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
    public Vector2 TargetPosition;
    public EDirection MoveInDirection;
    public float MoveDuration;
    public float StayInterval;
    public bool IsAttack;
}
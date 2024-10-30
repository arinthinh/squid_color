using System.Collections.Generic;
using UnityEngine;

public class SquidConfigSO : ScriptableObject
{
    [Header("MOVE")]
    public float MaxMoveIndex = 6;
    public float MoveDistance = 2f;
    public float MoveDuration = 0.25f;

    [Header("SHOOT")]
    public float ShootCooldown = 0.2f;
    public float StunTime = 1f;
    public float InvisibleTime = 0.5f;
    public int MaxInkAmount;
    public float InkReloadTime;
    public List<EColor> StartedInks;
}
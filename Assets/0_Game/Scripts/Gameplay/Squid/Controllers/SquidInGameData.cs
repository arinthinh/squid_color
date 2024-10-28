using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class SquidInGameData
{
    public SquidInGameData(List<LevelConfig.SquidInkConfig> startedInk)
    {
        Position = 0;
        foreach (var ink in startedInk)
        {
            Inks.Add(ink.Color, ink.Count);
        }
    }

    public int Position;
    public Dictionary<EColor, int> Inks = new();

    public bool IsOutOfInk => Inks.All(ink => ink.Value <= 0);
    public EColor CurColor;

    public EColor GetValidInk()
    {
        return !IsOutOfInk ? Inks.First(ink => ink.Value > 0).Key : EColor.White;
    }
}
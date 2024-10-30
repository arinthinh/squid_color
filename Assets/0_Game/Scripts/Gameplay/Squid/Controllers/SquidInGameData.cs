using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class SquidInGameData
{
    public SquidInGameData(List<EColor> startedInk, int maxInkAmount)
    {
        Position = 0;
        foreach (var ink in startedInk)
        {
            Inks.Add(ink, maxInkAmount);
        }
        CurColor = Inks.First().Key;
    }

    public int Position;
    public Dictionary<EColor, int> Inks = new();
    public EColor CurColor;
}
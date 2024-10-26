using System;
using System.Collections.Generic;
using System.Linq;

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
    public List<EColor> EmemyColors;
    public List<SquidInkConfig> SquidStartedInks;
    public List<TargetConfig> Targets;

    [Serializable]
    public class TargetConfig
    {
        public EColor Color;
        public int Targets;
    }

    [Serializable]
    public class SquidInkConfig
    {
        public EColor Color;
        public int Count;
    }
}
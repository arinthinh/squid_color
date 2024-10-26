using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ColorMixFormulaConfigSO : ScriptableObject
{
    public List<ColorMixFormulaConfig> Configs;
    
    public EColor GetResult(EColor color1, EColor color2)
    {
        var resultConfig = Configs.FirstOrDefault(config => config.Colors.Contains(color1) && config.Colors.Contains(color2));
        return resultConfig?.Result ?? EColor.White;
    }
}

[Serializable]
public class ColorMixFormulaConfig
{
    public List<EColor> Colors;
    public EColor Result;
}
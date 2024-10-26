using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ColorDefineSO : ScriptableObject
{
    public List<ColorDefine> Colors;

    public ColorDefine GetColor(EColor key)
    {
        return Colors.FirstOrDefault(c => c.Key == key) ?? Colors[0];
    }
}

[Serializable]
public class ColorDefine
{
    public EColor Key;
    public string RGB;
    public Color Color;
}
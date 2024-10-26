
using UnityEngine;

public abstract class SquidController : MonoBehaviour
{
    protected SquidConfigSO _config;
    protected SquidInGameData _data;

    public virtual void LoadData(SquidConfigSO config, SquidInGameData inGameData)
    {
        _data = inGameData;
        _config = config;
    }

    public virtual void Init()
    {
        
    }

    public virtual void Enable()
    {
        
    }

    public virtual void Disable()
    {
        
    }
}

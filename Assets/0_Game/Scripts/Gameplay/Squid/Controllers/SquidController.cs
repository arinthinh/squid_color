
using UnityEngine;

public abstract class SquidController : MonoBehaviour
{
    protected SquidConfigSO _config;
    protected SquidInGameData _data;

    public virtual void Init(SquidConfigSO config, SquidInGameData inGameData)
    {
        _data = inGameData;
        _config = config;
    }

    public virtual void OnStartPlay()
    {
        
    }

    public virtual void OnStopPlay()
    {
        
    }
}

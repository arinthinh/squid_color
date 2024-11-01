using System.Collections.Generic;
using UnityEngine;

public class Squid : MonoBehaviour
{
    [Header("DATA")]
    [SerializeField] private SquidConfigSO _config;
    [SerializeField] private SquidInGameData _inGameData;

    [Header("CONTROLLERS")]
    [SerializeField] private List<SquidController> _controllers;

    public void Init()
    {
        _inGameData = new(_config.StartedInks, _config.MaxInkAmount);
        _controllers.ForEach(c => c.Init(_config, _inGameData));
    }
    
    public void OnStartPlay() => _controllers.ForEach(c => c.OnStartPlay());
    
    public void OnStopPlay() => _controllers.ForEach(c => c.OnStopPlay());
}
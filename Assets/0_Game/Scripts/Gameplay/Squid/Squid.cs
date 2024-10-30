using System.Collections.Generic;
using UnityEngine;

public class Squid : MonoBehaviour
{
    [Header("DATA")]
    [SerializeField] private SquidConfigSO _config;
    [SerializeField] private SquidInGameData _inGameData;

    [Header("CONTROLLERS")]
    [SerializeField] private List<SquidController> _controllers;

    public void OnStartPlay()
    {
        _inGameData = new(_config.StartedInks, _config.MaxInkAmount);

        _controllers.ForEach(c =>
        {
            c.LoadData(_config, _inGameData);
            c.OnStartPlay();
        });
    }

    public void OnStopPlay()
    {
        _controllers.ForEach(crl => crl.OnStopPlay());
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public static Action WinLevel;
    public static Action LoseLevel;

    [Header("LEVEL DATA")]
    [SerializeField] private LevelData _curLevelData;

    [Header("LEVEL UNITS")]
    [SerializeField] private Squid _squid;
    [SerializeField] private EnemyManager _enemyManager;

    private LevelConfig _config;
    private ITargetPresenter _targetPresenter;
    private ITimerPresenter _timerPresenter;
    private readonly Timer _timer = new();

    private void OnEnable()
    {
        _timer.TimeChanged += OnTimerChanged;
        _timer.TimeUp += OnLoseLevel;
        _enemyManager.EnemyDie += OnEnemyDie;
    }

    private void OnDisable()
    {
        _timer.TimeChanged -= OnTimerChanged;
        _timer.TimeUp -= OnLoseLevel;
        _enemyManager.EnemyDie -= OnEnemyDie;
    }

    public void StartLevel(int levelIndex)
    {
        var inGameUIView = UIManager.Instance.GetView<InGameUIView>();

        // Init data
        _config = ConfigManager.Instance.GetConfig<LevelConfigCollectionSO>().GetLevelConfig(levelIndex);
        _curLevelData = new(_config.Targets, _config.LevelTime);
        _targetPresenter = inGameUIView;
        _timerPresenter = inGameUIView;

        // Enable gameplay units
        _timer.Start(_curLevelData.SecondLeft);
        _timerPresenter.ShowTimer(_curLevelData.SecondLeft);
        _targetPresenter.LoadTargetsInfo(_curLevelData.Targets);
        _targetPresenter.LoadTargetsInfo(_curLevelData.Targets);
        _squid.OnStartPlay(_config.SquidStartedInks);
        _enemyManager.OnStartPlay(_config.EmemyColors, _config.EnemyWaves);
    }

    private void CheckWin()
    {
        if (_curLevelData.Targets.All(t => t.Target <= 0))
        {
            OnWinLevel();
        }
    }

    private void StopPlay()
    {
        _timer.Stop();
        _squid.OnStopPlay();
        _enemyManager.OnStopPlay();
    }

    private void OnWinLevel()
    {
        StopPlay();
        GameDataController.Instance.OnWinLevel();
        UIManager.Instance.GetView<WinUIView>().Show();
        WinLevel?.Invoke();
    }

    private void OnLoseLevel()
    {
        StopPlay();
        UIManager.Instance.GetView<LoseUIView>().Show();
        LoseLevel?.Invoke();
    }

    private void OnTimerChanged(int secondLeft)
    {
        _curLevelData.SecondLeft = secondLeft;
        _timerPresenter.UpdateTimer(secondLeft);
    }

    private void OnEnemyDie(EColor color, Vector3 position)
    {
        var targetData = _curLevelData.GetTarget(color);
        if (targetData is not { Target: > 0 }) return;
        targetData.Target--;
        _targetPresenter.UpdateTargetInfo(targetData.Color, targetData.Target);
        CheckWin();
    }
}

[Serializable]
public class LevelData
{
    public int SecondLeft;
    public List<TargetData> Targets = new();

    public LevelData(List<LevelConfig.TargetConfig> targetConfigs, int levelTime)
    {
        SecondLeft = levelTime;

        foreach (var cfg in targetConfigs)
        {
            Targets.Add(new(cfg.Color, cfg.Targets));
        }
    }

    public TargetData GetTarget(EColor color)
    {
        return Targets.FirstOrDefault(t => t.Color == color);
    }

    [Serializable]
    public class TargetData
    {
        public TargetData(EColor color, int target)
        {
            Color = color;
            Target = target;
        }

        public EColor Color;
        public int Target;
    }
}
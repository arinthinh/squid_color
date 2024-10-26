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
        _timer.TimeUp += OnTimeUp;
        _enemyManager.EnemyDie += OnEnemyDie;
    }

    private void OnDisable()
    {
        _timer.TimeChanged -= OnTimerChanged;
        _timer.TimeUp -= OnTimeUp;
        _enemyManager.EnemyDie -= OnEnemyDie;
    }

    public void StartLevel(int levelIndex, InGameUIView inGameUIView)
    {
        _config = ConfigManager.Instance.GetConfig<LevelConfigCollectionSO>().GetLevelConfig(levelIndex);
        _curLevelData = new(_config.Targets, _config.LevelTime);

        _timer.Start(_curLevelData.SecondLeft);

        _targetPresenter = inGameUIView;
        _timerPresenter = inGameUIView;

        _timerPresenter.ShowTimer(_curLevelData.SecondLeft);
        _targetPresenter.LoadTargetInfo(_curLevelData.Targets);

        _squid.OnStartPlay(_config.SquidStartedInks);
    }

    private void CheckWin()
    {
        if (_curLevelData.Targets.All(t => t.Target <= 0))
        {
            OnWinLevel();
        }
    }

    private void OnWinLevel()
    {
        // Save data
        GameDataController.Instance.OnWinLevel();
        // Display UI
        UIManager.Instance.GetView<WinUIView>().Show();
        // Raise event
        WinLevel?.Invoke();
    }

    private void OnTimerChanged(int secondLeft)
    {
        _curLevelData.SecondLeft = secondLeft;
        _timerPresenter.UpdateTimer(secondLeft);
    }

    private void OnTimeUp()
    {
        LoseLevel?.Invoke();
    }

    private void OnEnemyDie(EColor color)
    {
        var targetData = _curLevelData.GetTarget(color);
        targetData.Target--;
        _targetPresenter.OnTargetInfoChanged(targetData.Color, targetData.Target, Vector3.zero);
        CheckWin();
    }
}

[Serializable]
public class LevelData
{
    public LevelData(List<LevelConfig.TargetConfig> targetConfigs, int levelTime)
    {
        SecondLeft = levelTime;

        foreach (var cfg in targetConfigs)
        {
            Targets.Add(new(cfg.Color, cfg.Targets));
        }
    }

    public int SecondLeft;
    public List<TargetData> Targets = new();

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
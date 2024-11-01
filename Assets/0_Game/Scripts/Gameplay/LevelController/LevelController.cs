using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    [Header("LEVEL DATA")]
    [SerializeField] private LevelData _curLevelData;

    [Header("LEVEL UNITS")]
    [SerializeField] private Squid _squid;
    [SerializeField] private StarfishsManager _starfishsManager;

    private LevelConfig _config;
    private ITargetPresenter _targetPresenter;
    private ITimerPresenter _timerPresenter;
    private readonly Timer _timer = new();

    private void OnEnable()
    {
        _timer.TimeChanged += OnTimerChanged;
        _timer.TimeUp += OnLoseLevel;
        _starfishsManager.EnemyDie += StarfishsDie;
    }

    private void OnDisable()
    {
        _timer.TimeChanged -= OnTimerChanged;
        _timer.TimeUp -= OnLoseLevel;
        _starfishsManager.EnemyDie -= StarfishsDie;
    }

    public async UniTaskVoid StartLevel(int levelIndex)
    {
        await UniTask.Yield();
        var inGameUIView = UIManager.Instance.GetView<InGameUIView>();

        // Init data
        _config = ConfigManager.Instance.GetConfig<LevelConfigCollectionSO>().GetLevelConfig(levelIndex);
        _curLevelData = new(levelIndex,_config.Targets, _config.LevelTime);
        _targetPresenter = inGameUIView;
        _timerPresenter = inGameUIView;
        
        // Init view
        _timerPresenter.ShowTimer(_curLevelData.SecondLeft);
        _targetPresenter.LoadTargetsInfo(_curLevelData.Targets);
        
        // Init units
        _squid.Init();
        _starfishsManager.Init();
        
        // Play countdown
        UIManager.Instance.GetView<CountdownUIView>().Show();
        await UniTask.WaitForSeconds(4f);

        // Enable gameplay units
        _timer.Start(_curLevelData.SecondLeft);
        _squid.OnStartPlay();
        _starfishsManager.OnStartPlay(_config.EnemyWaves);
    }

    private void CheckWin()
    {
        if (_curLevelData.Targets.All(t => t.Target <= 0))
        {
            OnWinLevel();
        }
    }

    public void StopPlay()
    {
        _timer.Stop();
        _squid.OnStopPlay();
        _starfishsManager.OnStopPlay();
    }
    
    public void ExitLevel()
    {
        _starfishsManager.OnExit();
    }

    private void OnWinLevel()
    {
        StopPlay();
        GameDataController.Instance.OnWinLevel(_curLevelData.LevelIndex);
        UIManager.Instance.GetView<WinUIView>().Show();
    }

    private void OnLoseLevel()
    {
        StopPlay();
        UIManager.Instance.GetView<LoseUIView>().Show();
    }

    private void OnTimerChanged(int secondLeft)
    {
        _curLevelData.SecondLeft = secondLeft;
        _timerPresenter.UpdateTimer(secondLeft);
    }

    private void StarfishsDie(EColor color)
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
    public int LevelIndex;
    public int SecondLeft;
    public List<TargetData> Targets = new();

    public LevelData(int levelIndex, List<LevelConfig.TargetConfig> targetConfigs, int levelTime)
    {
        LevelIndex = levelIndex;
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
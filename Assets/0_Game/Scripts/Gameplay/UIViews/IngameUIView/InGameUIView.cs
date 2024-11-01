using System;
using System.Collections.Generic;
using System.Linq;
using JSAM;
using TMPro;
using Toolkit.UI;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public class InGameUIView : UIView, ITargetPresenter, ITimerPresenter
{
    [SerializeField] private RectTransform _targetPanelsContainer;
    [SerializeField] private TargetInfoUIPanel _targetInfoUIPanelPrefab;
    [SerializeField] private TextMeshProUGUI _timerTMP;
    [SerializeField] private Button _settingsButton;

    private ObjectPool<TargetInfoUIPanel> _targetInfoUIPool;
    private readonly List<TargetInfoUIPanel> _targetInfoUIPanels = new();

    private void OnEnable()
    {
        _settingsButton.onClick.AddListener(OnSettingsButtonClick);
    }

    private void OnDisable()
    {
        _settingsButton.onClick.RemoveListener(OnSettingsButtonClick);
    }

    private void Start()
    {
        _targetInfoUIPool = UnityObjectPoolCreator.CreatePool(_targetInfoUIPanelPrefab, _targetPanelsContainer);
    }

    public override void Show()
    {
        base.Show();
        _timerTMP.gameObject.SetActive(false);
    }

    public override void Hide()
    {
        base.Hide();
        _targetInfoUIPanels.ForEach(t =>
        {
            if (t.gameObject.activeSelf) _targetInfoUIPool.Release(t);
        });
        _targetInfoUIPanels.Clear();
    }

    public void LoadTargetsInfo(List<LevelData.TargetData> targetDatas)
    {
        foreach (var data in targetDatas)
        {
            var newTargetInfoUIPanel = _targetInfoUIPool.Get();
            _targetInfoUIPanels.Add(newTargetInfoUIPanel);
            newTargetInfoUIPanel.OnSpawn(data.Color);
            newTargetInfoUIPanel.UpdateTargetCount(data.Target);
        }
    }

    public void UpdateTargetInfo(EColor color, int newValue)
    {
        var panel = GetTargetInfoUIPanel(color);
        if (panel == null) return;
        panel.UpdateTargetCount(newValue, true);
    }

    private TargetInfoUIPanel GetTargetInfoUIPanel(EColor color)
    {
        return _targetInfoUIPanels.FirstOrDefault(t => t.Color == color);
    }

    public void ShowTimer(int secondLeft)
    {
        _timerTMP.gameObject.SetActive(true);
        _timerTMP.text = $"{secondLeft}";
    }

    public void UpdateTimer(int secondLeft)
    {
        _timerTMP.text = $"{secondLeft}";
    }

    private void OnSettingsButtonClick()
    {
        AudioManager.PlaySound(ESound.ClickSoundSO);
        UIManager.Instance.GetView<SettingsUIView>().Show(true);
    }
}

public interface ITargetPresenter
{
    void LoadTargetsInfo(List<LevelData.TargetData> targetDatas);
    void UpdateTargetInfo(EColor color, int newValue);
}

public interface ITimerPresenter
{
    void ShowTimer(int secondLeft);
    void UpdateTimer(int secondLeft);
}
using System.Collections.Generic;
using TMPro;
using Toolkit.UI;
using UnityEngine;

public class InGameUIView : UIView, ITargetPresenter, ITimerPresenter
{
    [SerializeField] private TextMeshProUGUI _timerTMP;
    
    public override void Show()
    {
        base.Show();
        _timerTMP.gameObject.SetActive(false);
    }

    public void LoadTargetInfo(List<LevelData.TargetData> targetDatas)
    {
    }

    public void OnTargetInfoChanged(EColor color, int newValue, Vector3 fxStartPosition)
    {
        // Update UI
        // Play animation fly color to target panel
    }

    public void ShowTimer(int secondLeft)
    {
        _timerTMP.gameObject.SetActive(true);
        _timerTMP.text = $"{secondLeft}s";
    }

    public void UpdateTimer(int secondLeft)
    {
        _timerTMP.text = $"{secondLeft}s";
    }
}

public interface ITargetPresenter
{
    void LoadTargetInfo(List<LevelData.TargetData> targetDatas);
    void OnTargetInfoChanged(EColor color, int newValue, Vector3 fxStartPosition);
}

public interface ITimerPresenter
{
    void ShowTimer(int secondLeft);
    void UpdateTimer(int secondLeft);
}
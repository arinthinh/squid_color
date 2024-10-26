using System;
using DG.Tweening;

[Serializable]
public class Timer
{
    public event Action<int> TimeChanged;
    public event Action TimeUp;

    public bool IsRunning;
    public int SecondLeft;

    private Tween _timerTween;

    /// <summary>
    /// Start a new timer.
    /// </summary>
    public void Start(int second)
    {
        IsRunning = true;
        SecondLeft = second;
        _timerTween?.Kill();
        _timerTween = DOVirtual
            .Int(second, 0, second, value =>
            {
                SecondLeft = value;
                TimeChanged?.Invoke(SecondLeft);
            })
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                Stop();
                TimeUp?.Invoke();
            });
    }

    public void Stop()
    {
        _timerTween?.Kill();
        SecondLeft = 0;
        IsRunning = false;
    }
}
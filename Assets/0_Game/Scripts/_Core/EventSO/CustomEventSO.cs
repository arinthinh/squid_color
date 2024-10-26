using System;
using UnityEngine.Events;

public class CustomEventSO<T> : EventSO
{
    private event Action<T> _event;

    public void Raise(T param)
    {
        _event?.Invoke(param);
    }

    public void AddListener(Action<T> action)
    {
        _event += action;
    }
    
    public void RemoveListener(Action<T> action)
    {
        _event -= action;
    }
    
    private void OnDestroy()
    {
        _event = null;
    }
}

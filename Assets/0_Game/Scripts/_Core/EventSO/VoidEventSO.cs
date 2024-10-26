using System;
using UnityEngine;
using UnityEngine.Events;

public class VoidEventSO : EventSO
{
    private event Action _event;

    /// <summary>
    /// Raise the event
    /// </summary>
    public void Raise()
    {
        _event?.Invoke();
    }

    /// <summary>
    /// Add a listener to the event
    /// </summary>
    /// <param name="action">The action to add</param>
    public void AddListener(Action action)
    {
        _event += action;
    }

    /// <summary>
    /// Remove a listener from the event
    /// </summary>
    /// <param name="action">The action to remove</param>
    public void RemoveListener(Action action)
    {
        _event -= action;
    }

    /// <summary>
    /// When the object is disabled, all listeners are removed
    /// </summary>
    private void OnDisable()
    {
        _event = null;
    }
}
using UnityEngine;

public abstract class EventSO : ScriptableObject
{
#if UNITY_EDITOR
    [TextArea]
    public string Description;
#endif
}
using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SquidChangeColorButton : MonoBehaviour
{
    public static Action<EColor> Clicked;

    private int _index;
    private Button _button;
    private EColor _color;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(OnButtonClick);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        Clicked?.Invoke(_color);
    }
}
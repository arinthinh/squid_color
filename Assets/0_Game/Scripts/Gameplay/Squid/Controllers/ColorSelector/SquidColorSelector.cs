using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class SquidColorSelector : SquidController
{
    [SerializeField] private SquidAvatarController _avatarController;
    [SerializeField] private List<SquidChangeColorButton> _buttons;

    [SerializeField] private InputActionReference _changeColorButton1;
    [SerializeField] private InputActionReference _changeColorButton2;
    [SerializeField] private InputActionReference _changeColorButton3;

    private void OnEnable()
    {
        _changeColorButton1.action.started += OnChangeColorButton1;
        _changeColorButton2.action.started += OnChangeColorButton2;
        _changeColorButton3.action.started += OnChangeColorButton3;
        SquidChangeColorButton.Clicked += ChangeColor;
    }

    private void OnDisable()
    {
        _changeColorButton1.action.started -= OnChangeColorButton1;
        _changeColorButton2.action.started -= OnChangeColorButton2;
        _changeColorButton3.action.started -= OnChangeColorButton3;
        SquidChangeColorButton.Clicked -= ChangeColor;
    }


    public override void OnStartPlay()
    {
        base.OnStartPlay();
        UpdateDisplayAmount();
        ChangeColor(_data.CurColor);
    }

    public void ChangeColor(EColor color)
    {
        _data.CurColor = color;
        _avatarController.ChangeColor(color);
    }

    public void PlayReloadAnimation(EColor inkColor, float reloadTime)
    {
        var btn = GetButton(inkColor);
        if (btn == null) return;
        btn.PlayReloadAnimation(reloadTime);
    }

    public void UpdateDisplayAmount()
    {
        foreach (var inkData in _data.Inks)
        {
            var btn = GetButton(inkData.Key);
            if (btn == null) continue;

            btn.UpdateDisplayAmount(inkData.Value, _config.MaxInkAmount);
        }
    }

    private SquidChangeColorButton GetButton(EColor color)
    {
        return _buttons.FirstOrDefault(b => b.Color == color);
    }

    private SquidChangeColorButton GetButton(int index)
    {
        return _buttons.FirstOrDefault(b => b.Index == index);
    }

    private void OnChangeColorButton1(InputAction.CallbackContext context) => OnInputKeyButton(1);
    private void OnChangeColorButton2(InputAction.CallbackContext context) => OnInputKeyButton(2);
    private void OnChangeColorButton3(InputAction.CallbackContext context) => OnInputKeyButton(3);

    private void OnInputKeyButton(int input)
    {
        var btn = GetButton(input);
        if (btn == null) return;
        ChangeColor(btn.Color);
    }
}
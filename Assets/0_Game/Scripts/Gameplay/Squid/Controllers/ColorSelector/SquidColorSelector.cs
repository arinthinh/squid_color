using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class SquidColorSelector : SquidController
{
    [Header("COMPONENTS")]
    [SerializeField] private SquidStunController _stunController;
    [SerializeField] private SquidAnimator _animator;
    [SerializeField] private List<SquidChangeColorButton> _buttons;

    [Header("INPUT ACTIONS")]
    [SerializeField] private InputActionReference _changeColorButton1;
    [SerializeField] private InputActionReference _changeColorButton2;
    [SerializeField] private InputActionReference _changeColorButton3;

    private bool _isSwitching;
    public bool IsSwitching => _isSwitching;

    private void OnEnable()
    {
        _changeColorButton1.action.started += OnChangeColorButton1;
        _changeColorButton2.action.started += OnChangeColorButton2;
        _changeColorButton3.action.started += OnChangeColorButton3;
        SquidChangeColorButton.Clicked += OnChangeColorButtonClick;
    }

    private void OnDisable()
    {
        _changeColorButton1.action.started -= OnChangeColorButton1;
        _changeColorButton2.action.started -= OnChangeColorButton2;
        _changeColorButton3.action.started -= OnChangeColorButton3;
        SquidChangeColorButton.Clicked -= OnChangeColorButtonClick;
    }


    public override void OnStartPlay()
    {
        base.OnStartPlay();
        UpdateDisplayAmount();
        ChangeColorImmediately(_data.CurColor);
    }
    
    private void ChangeColorImmediately(EColor color)
    {
        _isSwitching = false;
        _data.CurColor = color;
        _animator.PlaySwitchColorAnimation(color, true);
    }

    private async UniTaskVoid PerformChangeColor(EColor color)
    {
        if (_stunController.IsStun) return;
        if (color == _data.CurColor) return;
        _isSwitching = true;
        _data.CurColor = color;
        _animator.PlaySwitchColorAnimation(color);
        await UniTask.WaitForSeconds(_config.SwitchColorTime);
        _isSwitching = false;
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

    private void OnChangeColorButtonClick(EColor color)
    {
        PerformChangeColor(color).Forget();
    }

    private void OnInputKeyButton(int input)
    {
        var btn = GetButton(input);
        if (btn == null) return;
        PerformChangeColor(btn.Color).Forget();
    }
}
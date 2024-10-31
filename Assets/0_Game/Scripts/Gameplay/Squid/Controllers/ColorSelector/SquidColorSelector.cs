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

    public override void Init(SquidConfigSO config, SquidInGameData inGameData)
    {
        base.Init(config, inGameData);
        ChangeColorImmediately(_data.CurColor);
        UpdateDisplayAmount();

        _changeColorButton1.action.Disable();
        _changeColorButton2.action.Disable();
        _changeColorButton3.action.Disable();
    }

    public override void OnStartPlay()
    {
        _changeColorButton1.action.Enable();
        _changeColorButton2.action.Enable();
        _changeColorButton3.action.Enable();
        base.OnStartPlay();
    }

    public override void OnStopPlay()
    {
        _changeColorButton1.action.Disable();
        _changeColorButton2.action.Disable();
        _changeColorButton3.action.Disable();
        base.OnStopPlay();
    }

    private void ChangeColorImmediately(EColor color)
    {
        _isSwitching = false;
        _data.CurColor = color;
        _animator.PlaySwitchColorAnimation(color, true);
        UpdateSelected();
    }

    private async UniTaskVoid PerformChangeColor(EColor color)
    {
        if (_isSwitching) return;
        if (_stunController.IsStun) return;
        if (color == _data.CurColor) return;
        _isSwitching = true;
        _data.CurColor = color;
        _animator.PlaySwitchColorAnimation(color);
        UpdateSelected();
        await UniTask.WaitForSeconds(_config.SwitchColorTime);
        _isSwitching = false;
    }

    private void UpdateSelected()
    {
        foreach (var btn in _buttons) btn.SetSelected(btn.Color == _data.CurColor);
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
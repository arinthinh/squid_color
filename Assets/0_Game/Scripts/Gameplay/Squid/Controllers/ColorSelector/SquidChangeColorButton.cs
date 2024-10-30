using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SquidChangeColorButton : MonoBehaviour
{
    public static Action<EColor> Clicked;

    [SerializeField] private int _index;
    [SerializeField] private EColor _color;
    [SerializeField] private Image _image;
    [SerializeField] private TextMeshProUGUI _amountTMP;

    private Button _button;
    public EColor Color => _color;
    public int Index => _index;

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

    public void PlayReloadAnimation(float reloadTime)
    {
        _image.DOFillAmount(1, reloadTime);
    }

    public void UpdateDisplayAmount(int inkDataValue, int maxInkValue)
    {
        _amountTMP.transform.DOKill();
        _amountTMP.transform.localScale = Vector3.one;
        _amountTMP.transform.DOPunchScale(Vector3.one * 0.1f, 0.25f);
        _amountTMP.text = inkDataValue.ToString();
        _image.fillAmount = (float)inkDataValue / maxInkValue;
    }
}
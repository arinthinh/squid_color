using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TargetInfoUIPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _doneTMP;
    [SerializeField] private Image _image;
    [SerializeField] private TextMeshProUGUI _targetCountTMP;
    [SerializeField] private StarfishSpritesConfigSO _starfishSpritesConfig;

    private EColor _curColor;
    public EColor Color => _curColor;

    public void OnSpawn(EColor color)
    {
        _curColor = color;
        _image.sprite = _starfishSpritesConfig.GetSprite(color, EStarfishState.Die);
        _targetCountTMP.gameObject.SetActive(true);
        _doneTMP.gameObject.SetActive(false);
    }

    public void UpdateTargetCount(int count, bool isPlayAnimation = false)
    {
        if (isPlayAnimation)
        {
            _targetCountTMP.transform.DOKill();
            _targetCountTMP.transform.localScale = Vector3.one;
            _targetCountTMP.transform.DOPunchScale(Vector3.one * 0.1f, 0.2f);
        }

        if (count <= 0)
        {
            _doneTMP.gameObject.SetActive(true);
            _targetCountTMP.gameObject.SetActive(false);
            return;
        }

        _targetCountTMP.text = count.ToString();
    }
}
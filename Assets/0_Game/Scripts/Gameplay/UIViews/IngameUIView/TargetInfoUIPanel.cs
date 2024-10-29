using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TargetInfoUIPanel : MonoBehaviour
{
    [SerializeField] private Image _doneImage;
    [SerializeField] private Image _image;
    [SerializeField] private TextMeshProUGUI _targetCountTMP;
    [SerializeField] private EnemySpritesConfigSO _enemySpritesConfig;

    private EColor _curColor;
    public EColor Color => _curColor;

    public void OnSpawn(EColor color)
    {
        _curColor = color;
        _image.sprite = _enemySpritesConfig.GetSprite(color, EEnemyState.Die);
        _targetCountTMP.gameObject.SetActive(true);
        _doneImage.gameObject.SetActive(false);
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
            _doneImage.gameObject.SetActive(true);
            _targetCountTMP.gameObject.SetActive(false);
            return;
        }

        _targetCountTMP.text = count.ToString();
    }
}
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Redcode.Extensions;
using TMPro;
using Toolkit.UI;
using UnityEngine;

public class CountdownUIView : UIView
{
    [SerializeField] private TextMeshProUGUI _cooldownText;
    
    public override void Show()
    {
        base.Show();
        PerformCountdown().Forget();
    }

    private async UniTaskVoid PerformCountdown()
    {
        _cooldownText.rectTransform.SetAnchoredPositionY(-100);
        _cooldownText.rectTransform.DOAnchorPosY(0, 0.5f);
        _cooldownText.text = "3";
        await UniTask.WaitForSeconds(1f);
        _cooldownText.rectTransform.SetAnchoredPositionY(-100);
        _cooldownText.rectTransform.DOAnchorPosY(0, 0.5f);
        _cooldownText.text = "2";
        await UniTask.WaitForSeconds(1f);
        _cooldownText.rectTransform.SetAnchoredPositionY(-100);
        _cooldownText.rectTransform.DOAnchorPosY(0, 0.5f);
        _cooldownText.text = "1";
        await UniTask.WaitForSeconds(1f);
        _cooldownText.rectTransform.SetAnchoredPositionY(-100);
        _cooldownText.rectTransform.DOAnchorPosY(0, 0.5f);
        _cooldownText.text = "START!";
        await UniTask.WaitForSeconds(1f);
        Hide();
    }
}